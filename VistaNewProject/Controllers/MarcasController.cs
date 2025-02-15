﻿using Microsoft.AspNetCore.Mvc;
using VistaNewProject.Models;
using VistaNewProject.Services;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace VistaNewProject.Controllers
{
    public class MarcasController : Controller
    {
        private readonly IApiClient _client;


        public MarcasController(IApiClient client)
        {
            _client = client;
        }



        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5; // Cambiado a 5 para que la paginación se haga cada 5 registros
            int pageNumber = page ?? 1; // Número de página actual (si no se especifica, es 1)

            var marcas = await _client.GetMarcaAsync(); // Obtener todas las marcas

            if (marcas == null)
            {
                return NotFound("error");
            }

            var pageMarca = await marcas.ToPagedListAsync(pageNumber, pageSize);
            if (!pageMarca.Any() && pageMarca.PageNumber > 1)
            {
                pageMarca = await marcas.ToPagedListAsync(pageMarca.PageCount, pageSize);
            }

            int contador = (pageNumber - 1) * pageSize + 1; // Calcular el valor inicial del contador

            ViewBag.Contador = contador;

            // Código del método Index que querías integrar
            string mensaje = HttpContext.Session.GetString("Message");
            TempData["Message"] = mensaje;

            try
            {
                ViewData["Marcas"] = marcas;
                return View(pageMarca);
            }
            catch (HttpRequestException ex) when ((int)ex.StatusCode == 404)
            {
                HttpContext.Session.SetString("Message", "No se encontró la página solicitada");
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                HttpContext.Session.SetString("Message", "Error en el aplicativo");
                return RedirectToAction("LogOut", "Accesos");
            }
        }
        public async Task<IActionResult> Details(int? id, int? page)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marcas = await _client.GetMarcaAsync();
            var marca = marcas.FirstOrDefault(u => u.MarcaId == id);
            if (marca == null)
            {
                return NotFound();
            }

            ViewBag.Marca = marca;

            var productos = await _client.GetProductoAsync();
            var productosDeMarca = productos.Where(p => p.MarcaId == id);

            int pageSize = 2; // Número máximo de elementos por página
            int pageNumber = page ?? 1;

            var pagedProductos = productosDeMarca.ToPagedList(pageNumber, pageSize);

            return View(pagedProductos);
        }

    [HttpPost]
        public async Task<IActionResult> Create([FromForm] string nombreMarca)
        {
            if (ModelState.IsValid)
            {
                var marcas = await _client.GetMarcaAsync();
                var marcasexis = marcas.FirstOrDefault(c => string.Equals(c.NombreMarca, nombreMarca, StringComparison.OrdinalIgnoreCase));

                // Si ya existe una categoría con el mismo nombre, mostrar un mensaje de error
                if (marcasexis != null)
                {
                    TempData["SweetAlertIcon"] = "error";
                    TempData["SweetAlertTitle"] = "Error";
                    TempData["SweetAlertMessage"] = "Ya hay una marca registrada con ese nombre.";
                    return RedirectToAction("Index");
                }

                // Resto del código para crear la nueva marca
                var marca = new Marca
                {
                    NombreMarca = nombreMarca
                };

                if ( marca==null)
                {
                    ViewBag.MensajeError = "No se pudieron campos  los datos.";
                    return View("Index");
                }

                var response = await _client.CreateMarcaAsync(marca);
              

                if (response.IsSuccessStatusCode)
                {
                    // Guardar un mensaje en TempData para mostrar en el Index
                    TempData["Mensaje"] = "¡Registro guardado correctamente!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.MensajeError = "No se pudieron guardar los datos.";
                    return View("Index");
                }
            }


            ViewBag.Mensaje = TempData["Mensaje"]; ViewBag.Mensaje = TempData["Mensaje"];
            return View("Index");
        }
        public async Task<IActionResult> Update([FromForm] int marcaIdAct, [FromForm] string nombreMarcaAct, [FromForm] int estadoMarcaAct)
        {
            try
            {
                var marcas = await _client.GetMarcaAsync();
                var marcasExis = marcas.FirstOrDefault(c =>
                            string.Equals(c.NombreMarca, nombreMarcaAct, StringComparison.OrdinalIgnoreCase)
                            && c.MarcaId != marcaIdAct);
                // Si ya existe una categoría con el mismo nombre, mostrar un mensaje de error
                if (marcasExis != null)
                {
                    TempData["SweetAlertIcon"] = "error";
                    TempData["SweetAlertTitle"] = "Error";
                    TempData["SweetAlertMessage"] = "Ya hay una marca registrada con ese nombre.";
                    return RedirectToAction("Index");
                }
                // Continuar con la lógica de actualización de la marca si no hay una marca con el mismo nombre

                // Crear un objeto Marca con los datos recibidos del formulario
                var marca = new Marca
                {
                    MarcaId = marcaIdAct,
                    NombreMarca = nombreMarcaAct,
                    EstadoMarca = estadoMarcaAct == 1 ? 1ul : 0ul
                };

                // Llamar al método en el cliente para actualizar la marca
                var response = await _client.UpdateMarcaAsync(marca);

                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SweetAlertIcon"] = "success";
                        TempData["SweetAlertTitle"] = "Éxito";
                        TempData["SweetAlertMessage"] = "Marca actualizada correctamente.";
                        return RedirectToAction("Index");
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        TempData["SweetAlertIcon"] = "error";
                        TempData["SweetAlertTitle"] = "Error";
                        TempData["SweetAlertMessage"] = "La marca no se encontró en el servidor.";
                        return RedirectToAction("Index");
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        TempData["SweetAlertIcon"] = "error";
                        TempData["SweetAlertTitle"] = "Error";
                        TempData["SweetAlertMessage"] = "Nombre de marca duplicado.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["SweetAlertIcon"] = "error";
                        TempData["SweetAlertTitle"] = "Error";
                        TempData["SweetAlertMessage"] = "Error al actualizar la marca.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["SweetAlertIcon"] = "error";
                    TempData["SweetAlertTitle"] = "Error";
                    TempData["SweetAlertMessage"] = "Error al realizar la solicitud de actualización.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante la actualización
                TempData["SweetAlertIcon"] = "error";
                TempData["SweetAlertTitle"] = "Error";
                TempData["SweetAlertMessage"] = "Error al actualizar la marca: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var productos = await _client.GetProductoAsync();
            var productosDeMarca = productos.Where(p => p.MarcaId == id);

            if (productosDeMarca.Any())
            {
                TempData["SweetAlertIcon"] = "error";
                TempData["SweetAlertTitle"] = "Error";
                TempData["SweetAlertMessage"] = "No se puede eliminar la Marca porque tiene productos asociados.";
                return RedirectToAction("Index");
            }

            var response = await _client.DeleteMarcaAsync(id);
            if (response == null)
            {
                TempData["SweetAlertIcon"] = "error";
                TempData["SweetAlertTitle"] = "Error";
                TempData["SweetAlertMessage"] = "Error al eliminar la Marca.";
            }
            else if (response.IsSuccessStatusCode)
            {
                TempData["SweetAlertIcon"] = "success";
                TempData["SweetAlertTitle"] = "Éxito";
                TempData["SweetAlertMessage"] = "Marca eliminada correctamente.";
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                TempData["SweetAlertIcon"] = "error";
                TempData["SweetAlertTitle"] = "Error";
                TempData["SweetAlertMessage"] = "La Marca no se encontró en el servidor.";
            }
            else
            {
                TempData["SweetAlertIcon"] = "error";
                TempData["SweetAlertTitle"] = "Error";
                TempData["SweetAlertMessage"] = "Error desconocido al eliminar la Marca.";
            }

            return RedirectToAction("Index");
        }



    }


}
