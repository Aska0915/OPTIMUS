﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using VistaNewProject.Models;
using VistaNewProject.Services;
using X.PagedList;

namespace VistaNewProject.Controllers
{

    public class UsuariosController : Controller
    {
        private readonly IApiClient _client;


        public UsuariosController(IApiClient client)
        {
            _client = client;
        }


        public async Task<ActionResult> Index(int? page)
        {
            int pageSize = 5; // Número máximo de elementos por página
            int pageNumber = page ?? 1;

            var roles = await _client.GetRolAsync();

            var usuarios = await _client.GetUsuarioAsync();

            if (usuarios == null)
            {
                return NotFound("error");
            }

            var pageUsuario = await usuarios.ToPagedListAsync(pageNumber, pageSize);
            if (!pageUsuario.Any() && pageUsuario.PageNumber > 1)
            {
                pageUsuario = await usuarios.ToPagedListAsync(pageUsuario.PageCount, pageSize);
            }

            int contador = (pageNumber - 1) * pageSize + 1; // Calcular el valor inicial del contador


            ViewBag.Contador = contador;
            ViewBag.Roles = roles;

            // Código del método Index que querías integrar
            string mensaje = HttpContext.Session.GetString("Message");
            TempData["Message"] = mensaje;

            try
            {
                ViewData["Usuarios"] = usuarios;
                return View(pageUsuario);
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

        public async Task<IActionResult> Details(int? id)
        {
            var usuarios = await _client.GetUsuarioAsync();
            var roles = await _client.GetRolAsync();
            if (id == null || usuarios == null)
            {
                return NotFound();
            }

            var usuario = usuarios.FirstOrDefault(u => u.UsuarioId == id);

            if (usuario == null)
            {
                return NotFound();
            }
            ViewBag.roles = roles;
            return View(usuario);
        }


        public async Task<IActionResult> Create([FromForm] int rolId, string nombre, string apellido, string usuario, string contraseña, string telefono, string correo, ulong estadoUsuario)
        {

            if (ModelState.IsValid)
            {



                var usuarios = await _client.GetUsuarioAsync();
                var usuariosExis = usuarios.FirstOrDefault(c => string.Equals(c.Usuario1, usuario, StringComparison.OrdinalIgnoreCase));

                if (usuariosExis != null)
                {
                    TempData["SweetAlertIcon"] = "error";
                    TempData["SweetAlertTitle"] = "Error";
                    TempData["SweetAlertMessage"] = "Ya hay un Usuario  registrado con ese nombre.";
                    return RedirectToAction("Index");
                }

                var Usuarios = new Usuario
                {

                    RolId = rolId,
                    Nombre = nombre,
                    Apellido = apellido,
                    Usuario1 = usuario,
                    Contraseña = contraseña,
                    Telefono = telefono,
                    Correo = correo,
                    EstadoUsuario = estadoUsuario

                };
                Console.WriteLine(Usuarios);
                var response = await _client.CreateUsuarioAsync(Usuarios);
                Console.WriteLine(response);


                if (response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "¡Registro guardado correctamente!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.MensajeError = "No se pudieron guardar los datos.";
                    return View("Index");
                }

            }

            return RedirectToAction("Index");


        }

        public async Task<IActionResult> Update([FromForm] int usuarioIdAct, int rolIdAct, string nombreAct, string apellidoAct, string usuarioAct, string contraseñaAct, string telefonoAct, string correoAct, ulong estadoUsuarioAct)
        {

            var usuarios = await _client.GetUsuarioAsync();
            var usuarioExis = usuarios.FirstOrDefault(c =>
                                     string.Equals(c.Usuario1, usuarioAct, StringComparison.OrdinalIgnoreCase)
                                     && c.UsuarioId != usuarioIdAct);
            // Si ya existe una categoría con el mismo nombre, mostrar un mensaje de error
            if (usuarioExis != null)
            {
                TempData["SweetAlertIcon"] = "error";
                TempData["SweetAlertTitle"] = "Error";
                TempData["SweetAlertMessage"] = "Ya hay un usuario  registrada con ese nombre.";
                return RedirectToAction("Index");
            }

            var Usuarios = new Usuario
            {

                UsuarioId = usuarioIdAct,
                RolId = rolIdAct,
                Nombre = nombreAct,
                Apellido = apellidoAct,
                Usuario1 = usuarioAct,
                Contraseña = contraseñaAct,
                Telefono = telefonoAct,
                Correo = correoAct,
                EstadoUsuario = estadoUsuarioAct
            };

            var response = await _client.UpdateUsuarioAsync(Usuarios);

            if (response != null)
            {

                if (response.IsSuccessStatusCode)
                {
                    TempData["SweetAlertIcon"] = "success";
                    TempData["SweetAlertTitle"] = "Éxito";
                    TempData["SweetAlertMessage"] = "Usuario actualizado correctamente.";
                    return RedirectToAction("Index");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    TempData["SweetAlertIcon"] = "error";
                    TempData["SweetAlertTitle"] = "Error";
                    TempData["SweetAlertMessage"] = "El Usuario no se encontró en el servidor.";
                    return RedirectToAction("Index");
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    TempData["SweetAlertIcon"] = "error";
                    TempData["SweetAlertTitle"] = "Error";
                    TempData["SweetAlertMessage"] = "Nombre de Usuario duplicado.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["SweetAlertIcon"] = "error";
                    TempData["SweetAlertTitle"] = "Error";
                    TempData["SweetAlertMessage"] = "Error al actualizar el Usuario.";
                    return RedirectToAction("Index");
                }
            }



            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var domicilios = await _client.GetDomicilioAsync();
            var domiciliosDelUsuario = domicilios.Where(d => d.UsuarioId == id);

            if (domiciliosDelUsuario.Any())
            {
                TempData["SweetAlertIcon"] = "error";
                TempData["SweetAlertTitle"] = "Error";
                TempData["SweetAlertMessage"] = "No se puede eliminar el Usuario  porque tiene Domicilios  asociados.";
                return RedirectToAction("Index");
            }

            var response = await _client.DeleteUsuarioAsync(id);
            if (response == null)
            {
                TempData["SweetAlertIcon"] = "error";
                TempData["SweetAlertTitle"] = "Error";
                TempData["SweetAlertMessage"] = "Error al eliminar el Usuario.";
            }
            else if (response.IsSuccessStatusCode)
            {
                TempData["SweetAlertIcon"] = "success";
                TempData["SweetAlertTitle"] = "Éxito";
                TempData["SweetAlertMessage"] = "Usuario eliminada correctamente.";
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                TempData["SweetAlertIcon"] = "error";
                TempData["SweetAlertTitle"] = "Error";
                TempData["SweetAlertMessage"] = "El Usuario no se encontró en el servidor.";
            }
            else
            {
                TempData["SweetAlertIcon"] = "error";
                TempData["SweetAlertTitle"] = "Error";
                TempData["SweetAlertMessage"] = "Error desconocido al eliminar el usuario.";
            }

            return RedirectToAction("Index");
        }


    }
}



