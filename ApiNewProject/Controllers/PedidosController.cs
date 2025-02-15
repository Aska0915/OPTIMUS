﻿
using ApiNewProject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiNewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly NewOptimusContext _context;

        public PedidosController(NewOptimusContext context)
        {
            _context = context;
        }


        [HttpGet("GetAllPedido")]

        public async Task<ActionResult<List<Pedido>>> GetAllPedido()
        {
            var List = await _context.Pedidos.Select(
                s => new Pedido
                {
                    PedidoId = s.PedidoId,
                    ClienteId = s.ClienteId,
                    TipoServicio = s.TipoServicio,
                    FechaPedido = s.FechaPedido,
                    EstadoPedido = s.EstadoPedido,
                    ValorTotalPedido = s.ValorTotalPedido,
                }
            ).ToListAsync();



            return List;

        }


        [HttpPost("InsertPedidos")]
        public async Task<IActionResult> InsertPedidos(Pedido pedido)
        {
            Console.WriteLine(pedido.TipoServicio);
            try
            {
                var vlPedido = new Pedido
                {
                    ClienteId = pedido.ClienteId,
                    TipoServicio = pedido.TipoServicio,
                    FechaPedido = pedido.FechaPedido,
                    EstadoPedido = pedido.EstadoPedido,
                    Domicilios = new List<Domicilio>() // Solo inicializamos los domicilios
                };

                if (pedido.TipoServicio == "Domicilio")
                {
                    foreach (var item in pedido.Domicilios)
                    {
                        vlPedido.Domicilios.Add(new Domicilio
                        {
                            PedidoId = vlPedido.PedidoId,
                            UsuarioId = item.UsuarioId,
                            Observacion = item.Observacion,
                            FechaEntrega = item.FechaEntrega,
                            DireccionDomiciliario = item.DireccionDomiciliario,
                            EstadoDomicilio = item.EstadoDomicilio
                        });
                    }
                }

                foreach (var item in pedido.Detallepedidos)
                {
                    if (!item.Cantidad.HasValue || !item.PrecioUnitario.HasValue)
                    {
                        return BadRequest("Cantidad y PrecioUnitario son requeridos.");
                    }

                    // Restar la cantidad del producto
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if (producto != null)
                    {
                        producto.CantidadTotal -= item.Cantidad.Value;

                        // Guardar los cambios en el producto
                        _context.Productos.Update(producto);
                        await _context.SaveChangesAsync();

                        // Restar la cantidad del lote más próximo a vencer
                        var lote = await _context.Lotes
                            .Where(l => l.ProductoId == item.ProductoId && l.Cantidad > 0)
                            .OrderBy(l => l.FechaVencimiento)
                            .FirstOrDefaultAsync();

                        if (lote != null)
                        {
                            if (lote.Cantidad >= item.Cantidad)
                            {
                                lote.Cantidad -= item.Cantidad.Value;
                            }
                            else
                            {
                                // Restar la cantidad del siguiente lote más viejo
                                var siguienteLote = await _context.Lotes
                                    .Where(l => l.ProductoId == item.ProductoId && l.Cantidad > 0 && l.FechaVencimiento > lote.FechaVencimiento)
                                    .OrderBy(l => l.FechaVencimiento)
                                    .FirstOrDefaultAsync();

                                if (siguienteLote != null && siguienteLote.Cantidad >= item.Cantidad)
                                {
                                    siguienteLote.Cantidad -= item.Cantidad.Value;
                                }
                                else
                                {
                                    throw new InvalidOperationException("No hay suficiente cantidad en los lotes disponibles para el producto.");
                                }
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("No hay lotes disponibles para el producto.");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("El producto no existe.");
                    }

                    vlPedido.Detallepedidos.Add(new Detallepedido
                    {
                        PedidoId = vlPedido.PedidoId,
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad.Value,
                        PrecioUnitario = item.PrecioUnitario.Value
                    });
                }

                _context.Pedidos.Add(vlPedido);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Error al insertar el pedido: " + ex.Message);
            }
        }



        [HttpGet("GetPedidos")]
        public async Task<ActionResult<Pedido>> GetPedidos(int id)
        {
            try
            {
                var pedido = await _context.Pedidos
                  
                    .FirstOrDefaultAsync(p => p.PedidoId == id);
                if (pedido == null)
                {
                    return NotFound();
                }
                return pedido;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "error del servidor");
            }
        }

        [HttpDelete("DeletePedido")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            try
            {
                var eliminarPedido = await _context.Pedidos.FindAsync(id);
                if (eliminarPedido == null)
                {
                    return NotFound();

                }
                var tienedetalles = await _context.Detallepedidos.AnyAsync(d => d.PedidoId == id);

                var tienedomicilio = await _context.Domicilios.AnyAsync(d => d.PedidoId == id);

                if (tienedetalles || tienedomicilio)
                {
                    return BadRequest("no se puede eliminar el pedido por que tiene datos asociados");
                }
                _context.Remove(eliminarPedido);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest("error en eliminar el pedido" + ex.Message);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "error en el servidor " + ex.Message);
            }
        }
        [HttpPut("UpdatePedido")]
        public async Task<IActionResult> UpdatePedido(int id, Pedido pedido)
        {
            try
            {
                // Buscar el pedido en la base de datos por su ID
                var pedidoExistente = await _context.Pedidos.FindAsync(id);
                if (pedidoExistente == null)
                {
                    return NotFound();
                }

                // Actualizar los campos del pedido existente con los valores del nuevo pedido
                pedidoExistente.ClienteId = pedido.ClienteId;
                pedidoExistente.TipoServicio = pedido.TipoServicio;
                pedidoExistente.FechaPedido = pedido.FechaPedido;
                pedidoExistente.EstadoPedido = pedido.EstadoPedido;
          

              
                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error del servidor al actualizar el pedido: " + ex.Message);
            }
        }

    }
}