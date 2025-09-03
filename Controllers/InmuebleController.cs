using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoInmobiliaria.Models;
using ProyectoInmobiliaria.Repository;
using System.Linq;

namespace ProyectoInmobiliaria.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly IInmuebleRepository _repo;
        private readonly IPropietarioRepository _repoPropietario;

        public InmuebleController(IInmuebleRepository repo, IPropietarioRepository repoPropietario)
        {
            _repo = repo;
            _repoPropietario = repoPropietario;
        }

        // GET: Inmueble
        public IActionResult Index()
        {
            var lista = _repo.Listar();
            return View(lista);
        }

        // GET: Inmueble/Details/5
        public IActionResult Details(int id)
        {
            var inmueble = _repo.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }

            // Validamos que PropietarioId tenga valor antes de llamar al repositorio
            if (inmueble.PropietarioId.HasValue)
            {
                inmueble.Propietario = _repoPropietario.ObtenerPorId(inmueble.PropietarioId.Value);
            }

            return View(inmueble);
        }

        // GET: Inmueble/Create
        public IActionResult Create()
        {
            var propietarios = _repoPropietario.Listar()
                .Select(p => new { p.IdPropietario, NombreCompleto = p.Apellido + ", " + p.Nombre });
            ViewBag.Propietarios = new SelectList(propietarios, "IdPropietario", "NombreCompleto");
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                _repo.Alta(inmueble);
                return RedirectToAction(nameof(Index));
            }

            var propietarios = _repoPropietario.Listar()
                .Select(p => new { p.IdPropietario, NombreCompleto = p.Apellido + ", " + p.Nombre });
            ViewBag.Propietarios = new SelectList(propietarios, "IdPropietario", "NombreCompleto", inmueble.PropietarioId);
            return View(inmueble);
        }

        // GET: Inmueble/Edit/5
        public IActionResult Edit(int id)
        {
            var inmueble = _repo.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }

            var propietarios = _repoPropietario.Listar()
                .Select(p => new { p.IdPropietario, NombreCompleto = p.Apellido + ", " + p.Nombre });
            ViewBag.Propietarios = new SelectList(propietarios, "IdPropietario", "NombreCompleto", inmueble.PropietarioId);
            return View(inmueble);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble)
        {
            if (id != inmueble.IdInmueble)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _repo.Modificar(inmueble);
                return RedirectToAction(nameof(Index));
            }

            var propietarios = _repoPropietario.Listar()
                .Select(p => new { p.IdPropietario, NombreCompleto = p.Apellido + ", " + p.Nombre });
            ViewBag.Propietarios = new SelectList(propietarios, "IdPropietario", "NombreCompleto", inmueble.PropietarioId);
            return View(inmueble);
        }

        // GET: Inmueble/Delete/5
        public IActionResult Delete(int id)
        {
            var inmueble = _repo.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }

            // Validamos que PropietarioId tenga valor antes de llamar al repositorio
            if (inmueble.PropietarioId.HasValue)
            {
                inmueble.Propietario = _repoPropietario.ObtenerPorId(inmueble.PropietarioId.Value);
            }

            return View(inmueble);
        }

        // POST: Inmueble/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Inmueble/PorPropietario/5
        public IActionResult PorPropietario(int id)
        {
            var lista = _repo.BuscarPorPropietario(id);
            return View("Index", lista);
        }
    }
}
