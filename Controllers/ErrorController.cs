using Microsoft.AspNetCore.Mvc;

namespace ProyectoInmobiliaria.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.Titulo = "Página no encontrada";
                    ViewBag.ErrorMessage = "La página que buscas no existe o fue eliminada.";
                    break;

                case 500:
                    ViewBag.Titulo = "Error interno del servidor";
                    ViewBag.ErrorMessage = "Ocurrió un error inesperado. Inténtalo de nuevo más tarde.";
                    break;

                case 403:
                    ViewBag.Titulo = "Acceso denegado";
                    ViewBag.ErrorMessage = "No tienes permisos para acceder a este recurso.";
                    break;

                default:
                    ViewBag.Titulo = "Error inesperado";
                    ViewBag.ErrorMessage = "Se produjo un problema al procesar tu solicitud.";
                    break;
            }

            return View("Error", statusCode);
        }
    }
}
