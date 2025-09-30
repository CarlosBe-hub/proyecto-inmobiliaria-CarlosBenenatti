using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria.Models;
using ProyectoInmobiliaria.Repository;

namespace ProyectoInmobiliaria.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly IPropietarioRepository _repo;
        private readonly IInmuebleRepository _repoInmueble;

        public PropietariosController(IPropietarioRepository repo, IInmuebleRepository repoInmueble)
        {
            _repo = repo;
            _repoInmueble = repoInmueble;
        }

        // GET: /Propietarios
        public IActionResult Index(string? q, bool? activos)
            => View(_repo.Listar(q, activos));

        // GET: /Propietarios/Details/5
        public IActionResult Details(int id)
        {
            var p = _repo.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // GET: /Propietarios/Create
        public IActionResult Create() => View(new Propietario());

        // POST: /Propietarios/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Propietario modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            var id = _repo.Alta(modelo);
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: /Propietarios/Edit/5
        public IActionResult Edit(int id)
        {
            var p = _repo.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: /Propietarios/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            modelo.IdPropietario = id;
            _repo.Modificar(modelo);
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: /Propietarios/Delete/5
        public IActionResult Delete(int id)
        {
            var p = _repo.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // POST: /Propietarios/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Propietarios/Propiedades/5
        public IActionResult Propiedades(int id)
        {
            var propietario = _repo.ObtenerPorId(id);
            if (propietario == null) return NotFound();

            var inmuebles = _repoInmueble.BuscarPorPropietario(id);

            ViewBag.Propietario = propietario;
            return View(inmuebles);
        }
    }
}
