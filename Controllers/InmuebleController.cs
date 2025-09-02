using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria.Models;
using ProyectoInmobiliaria.Repository;

namespace ProyectoInmobiliaria.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly IInmuebleRepository repo;

        public InmueblesController()
        {
            string connectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";
            repo = new InmuebleRepository(connectionString);
        }

        // GET: Inmuebles
        public IActionResult Index()
        {
            var lista = repo.Listar();
            return View(lista);
        }

        // GET: Inmuebles/Detalles
        public IActionResult Details(int id)
        {
            var inmueble = repo.ObtenerPorId(id);
            if (inmueble == null) return NotFound();
            return View(inmueble);
        }

        // GET: Inmuebles/Crear
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inmuebles/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                repo.Alta(inmueble);
                return RedirectToAction(nameof(Index));
            }
            return View(inmueble);
        }

        // GET: Inmuebles/Editar
        public IActionResult Edit(int id)
        {
            var inmueble = repo.ObtenerPorId(id);
            if (inmueble == null) return NotFound();
            return View(inmueble);
        }

        // POST: Inmuebles/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                repo.Modificar(inmueble);
                return RedirectToAction(nameof(Index));
            }
            return View(inmueble);
        }

        // GET: Inmuebles/Eliminar
        public IActionResult Delete(int id)
        {
            var inmueble = repo.ObtenerPorId(id);
            if (inmueble == null) return NotFound();
            return View(inmueble);
        }

        // POST: Inmuebles/Eliminar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
