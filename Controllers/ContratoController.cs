using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoInmobiliaria.Models;
using ProyectoInmobiliaria.Repository;

namespace ProyectoInmobiliaria.Controllers
{
    public class ContratosController : Controller
    {
        private readonly IContratoRepository repoContrato;
        private readonly IInmuebleRepository repoInmueble;
        private readonly IInquilinoRepository repoInquilino;

        public ContratosController()
        {
            string connectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";
            repoContrato = new ContratoRepository(connectionString);
            repoInmueble = new InmuebleRepository(connectionString);
            repoInquilino = new InquilinoRepository(connectionString);
        }

        // GET: Contratos
        public IActionResult Index()
        {
            var lista = repoContrato.Listar();
            return View(lista);
        }

        // GET: Contratos/Detalles
        public IActionResult Details(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        // GET: Contratos/Crear
        public IActionResult Create()
        {
            ViewBag.Inquilinos = new SelectList(repoInquilino.Listar(), "IdInquilino", "NombreCompleto");
            ViewBag.Inmuebles = new SelectList(repoInmueble.Listar(), "IdInmueble", "Direccion");
            return View();
        }

        // POST: Contratos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contrato contrato)
        {
            if (ModelState.IsValid)
            {
                repoContrato.Alta(contrato);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Inquilinos = new SelectList(repoInquilino.Listar(), "IdInquilino", "NombreCompleto", contrato.IdInquilino);
            ViewBag.Inmuebles = new SelectList(repoInmueble.Listar(), "IdInmueble", "Direccion", contrato.IdInmueble);

            return View(contrato);
        }

        // GET: Contratos/Editar
        public IActionResult Edit(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            ViewBag.Inquilinos = new SelectList(repoInquilino.Listar(), "IdInquilino", "NombreCompleto", contrato.IdInquilino);
            ViewBag.Inmuebles = new SelectList(repoInmueble.Listar(), "IdInmueble", "Direccion", contrato.IdInmueble);

            return View(contrato);
        }

        // POST: Contratos/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Contrato contrato)
        {
            if (ModelState.IsValid)
            {
                repoContrato.Modificar(contrato);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Inquilinos = new SelectList(repoInquilino.Listar(), "IdInquilino", "NombreCompleto", contrato.IdInquilino);
            ViewBag.Inmuebles = new SelectList(repoInmueble.Listar(), "IdInmueble", "Direccion", contrato.IdInmueble);

            return View(contrato);
        }

        // GET: Contratos/Eliminar
        public IActionResult Delete(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        // POST: Contratos/Eliminar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repoContrato.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
