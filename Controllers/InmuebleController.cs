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
        private readonly IContratoRepository _repoContrato;

        private const int TamPagina = 5; // cantidad de registros por página

        public InmuebleController(
            IInmuebleRepository repo,
            IPropietarioRepository repoPropietario,
            IContratoRepository repoContrato)
        {
            _repo = repo;
            _repoPropietario = repoPropietario;
            _repoContrato = repoContrato;
        }

        // GET: Inmueble
        public IActionResult Index(string estado, int pagina = 1)
        {
            var lista = _repo.Listar(pagina, TamPagina);

            if (!string.IsNullOrEmpty(estado))
            {
                lista = lista.Where(i => i.Estado == estado).ToList();
            }

            ViewBag.EstadoSeleccionado = estado;
            ViewBag.PaginaActual = pagina;
            ViewBag.TamPagina = TamPagina;

            // Podrías obtener el total de registros para calcular páginas
            var totalRegistros = _repo.Listar().Count;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / TamPagina);

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
            var contratos = _repoContrato.BuscarPorInmueble(id);

            if (contratos != null && contratos.Any())
            {
                TempData["Error"] = "⚠️ No se puede eliminar el inmueble porque tiene contratos asociados. Debe dar de baja primero esos contratos.";
                return RedirectToAction(nameof(Index));
            }

            _repo.Baja(id);
            TempData["Success"] = "✅ El inmueble fue eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Inmueble/Contratos/5
        public IActionResult Contratos(int id)
        {
            // Obtener los contratos asociados al inmueble
            var contratos = _repoContrato.BuscarPorInmueble(id);

            if (contratos == null || !contratos.Any())
            {
                TempData["Error"] = "No hay contratos asociados a este inmueble.";
                return RedirectToAction(nameof(Index));
            }

            // Devuelve los contratos a la vista
            return View(contratos);
        }

        // GET: Inmueble/PorPropietario/5
        public IActionResult PorPropietario(int id)
        {
            var lista = _repo.BuscarPorPropietario(id);
            return View("Index", lista);
        }
    }
}
