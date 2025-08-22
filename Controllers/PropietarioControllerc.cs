using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria.Models;
using ProyectoInmobiliaria.Repository;

namespace ProyectoInmobiliaria.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly IPropietarioRepository _repo;
        public PropietariosController(IPropietarioRepository repo) => _repo = repo;

        public IActionResult Index(string? q, bool? activos)
            => View(_repo.Listar(q, activos));

        public IActionResult Details(int id)
        {
            var p = _repo.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        public IActionResult Create() => View(new Propietario());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Propietario modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            var id = _repo.Alta(modelo);
            return RedirectToAction(nameof(Details), new { id });
        }

        public IActionResult Edit(int id)
        {
            var p = _repo.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario modelo)
        {
            if (!ModelState.IsValid) return View(modelo);
            modelo.IdPropietario = id;
            _repo.Modificar(modelo);
            return RedirectToAction(nameof(Details), new { id });
        }

        public IActionResult Delete(int id)
        {
            var p = _repo.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
