using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria.Models;
using ProyectoInmobiliaria.Repository;

namespace ProyectoInmobiliaria.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly IInquilinoRepository _repo;

        public InquilinosController(IInquilinoRepository repo)
        {
            _repo = repo;
        }

        // GET: Inquilinos
        public IActionResult Index(string? q, bool? activos)
        {
            var lista = _repo.Listar(q, activos);
            return View(lista);
        }

        // GET: Inquilinos/Details
        public IActionResult Details(int id)
        {
            var inq = _repo.ObtenerPorId(id);
            if (inq == null)
                return NotFound();

            return View(inq);
        }

        // GET: Inquilinos/Create
        public IActionResult Create()
        {
            return View(new Inquilino());
        }

        // POST: Inquilinos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var id = _repo.Alta(modelo);
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Inquilinos/Edit
        public IActionResult Edit(int id)
        {
            var inq = _repo.ObtenerPorId(id);
            if (inq == null)
                return NotFound();

            return View(inq);
        }

        // POST: Inquilinos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            modelo.IdInquilino = id;
            _repo.Modificar(modelo);

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Inquilinos/Delete
        public IActionResult Delete(int id)
        {
            var inq = _repo.ObtenerPorId(id);
            if (inq == null)
                return NotFound();

            return View(inq);
        }

        // POST: Inquilinos/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}