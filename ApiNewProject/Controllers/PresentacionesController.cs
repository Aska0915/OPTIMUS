﻿using ApiNewProject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ApiNewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresentacionesController : ControllerBase
    {
        private readonly NewOptimusContext _context;

        public PresentacionesController(NewOptimusContext context)
        {
            _context = context;
        }


        [HttpGet("GetPresentaciones")]
        public async Task<ActionResult<List<Presentacion>>> GetPresentacion()
        {
            var List = await _context.Presentaciones.Select(
                s => new Presentacion
                {
                    PresentacionId = s.PresentacionId,
                    NombrePresentacion = s.NombrePresentacion,
                    Contenido = s.Contenido,
                    CantidadPorPresentacion = s.CantidadPorPresentacion,
                    DescripcionPresentacion = s.DescripcionPresentacion,
                    EstadoPresentacion = s.EstadoPresentacion
                }
            ).ToListAsync();



            return List;

        }

        [HttpGet("GetPresentacionById")]
        public async Task<ActionResult<Presentacion>> GetPresentacionById(int Id)
        {

            Presentacion presentacion = await _context.Presentaciones.Select(
                    s => new Presentacion
                    {
                        PresentacionId = s.PresentacionId,
                        NombrePresentacion = s.NombrePresentacion,
                        DescripcionPresentacion = s.DescripcionPresentacion,
                        Contenido = s.Contenido,
                        CantidadPorPresentacion = s.CantidadPorPresentacion,
                        EstadoPresentacion = s.EstadoPresentacion
                    })
                .FirstOrDefaultAsync(s => s.PresentacionId == Id);

            if (presentacion == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(presentacion);
            }
        }


        [HttpPost("InsertarPresentacion")]
        public async Task<ActionResult<Presentacion>> InsertarPresentacion(Presentacion presentacion)
        {
            try
            {
                if (presentacion == null)
                {
                    return BadRequest("Los datos de la presentacion no pueden ser nulos.");
                }

                _context.Presentaciones.Add(presentacion);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPresentacionById), new { id = presentacion.PresentacionId }, presentacion);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al insertar la presentacion en la base de datos: " + ex.Message);
            }
        }


        [HttpPut("UpdatePresentaciones")]
        public async Task<ActionResult> UpdatePresentaciones(Presentacion presentacion)
        {
            var presentaciones = await _context.Presentaciones.FirstOrDefaultAsync(s => s.PresentacionId == presentacion.PresentacionId);

            if (presentaciones == null)
            {
                return NotFound();
            }
            presentaciones.PresentacionId = presentacion.PresentacionId;
            presentaciones.NombrePresentacion = presentacion.NombrePresentacion;
            presentaciones.DescripcionPresentacion = presentacion.DescripcionPresentacion;
            presentaciones.CantidadPorPresentacion = presentacion.CantidadPorPresentacion;
            presentaciones.Contenido = presentacion.Contenido; 
            presentaciones.EstadoPresentacion = presentacion.EstadoPresentacion;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("DeletePresentacion/{Id}")]
        public async Task<HttpStatusCode> DeletePresentacion(int Id)
        {
            var presentacion = new Presentacion()
            {
                PresentacionId = Id
            };
            _context.Presentaciones.Attach(presentacion);
            _context.Presentaciones.Remove(presentacion);
            await _context.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        [HttpPatch("UpdateEstadoPresentacion/{id}")]
        public async Task<IActionResult> UpdateEstadoPresentacion(int id, [FromBody] Presentacion EstadoPresentacion)
        {
            try
            {
                // Buscar el cliente por su ID
                var presentacion = await _context.Presentaciones.FindAsync(id);

                // Si no se encuentra el cliente, devolver un error 404 Not Found
                if (presentacion == null)
                {
                    return NotFound();
                }

                // Actualizar el estado del cliente con el nuevo valor
                presentacion.EstadoPresentacion = EstadoPresentacion.EstadoPresentacion;

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // Devolver una respuesta exitosa
                return Ok();
            }
            catch (Exception ex)
            {
                // Si ocurre algún error, devolver un error 500 Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar el estado del cliente: " + ex.Message);
            }
        }
    }
}
