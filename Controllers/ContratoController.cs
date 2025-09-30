using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoInmobiliaria.Models;
using ProyectoInmobiliaria.Repository;

namespace ProyectoInmobiliaria.Controllers
{
    public class ContratoController : Controller
    {
        private readonly IContratoRepository _repo;
        private readonly IInquilinoRepository _repoInquilino;
        private readonly IInmuebleRepository _repoInmueble;
        private readonly IPagoRepository _repoPago;

        public ContratoController(
            IContratoRepository repo,
            IInquilinoRepository repoInquilino,
            IInmuebleRepository repoInmueble,
            IPagoRepository repoPago)
        {
            _repo = repo;
            _repoInquilino = repoInquilino;
            _repoInmueble = repoInmueble;
            _repoPago = repoPago;
        }

        // GET: Contrato
        public IActionResult Index(int pageNumber = 1, int pageSize = 5)
        {
            var (contratos, totalCount) = _repo.ListarPaginado(pageNumber, pageSize);

            // Diccionario para saber si se puede renovar
            var renovables = contratos.ToDictionary(
                c => c.IdContrato,
                c => PuedeRenovarContrato(c.IdContrato)
            );

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Renovables = renovables;

            return View(contratos);
        }

        // GET: Contrato/Details
        public IActionResult Details(int id)
        {
            var contrato = _repo.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            return View(contrato);
        }

        // GET: Contrato/Create
        public IActionResult Create()
        {
            var inquilinos = _repoInquilino.Listar()
                .Select(i => new { i.IdInquilino, NombreCompleto = i.Nombre + " " + i.Apellido })
                .ToList();

            var inmuebles = _repoInmueble.Listar();

            ViewBag.Inquilinos = new SelectList(inquilinos, "IdInquilino", "NombreCompleto");
            ViewBag.Inmuebles = new SelectList(inmuebles, "IdInmueble", "Direccion");

            return View("Create");
        }

        // POST: Contrato/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contrato contrato)
        {
            if (contrato.FechaInicio >= contrato.FechaFin)
            {
                ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");
            }

            if (_repo.ExisteOcupacion(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin, contrato.IdContrato))
            {
                ModelState.AddModelError("", "El inmueble ya está ocupado en ese rango de fechas.");
            }

            if (ModelState.IsValid)
            {
                _repo.Alta(contrato);
                return RedirectToAction(nameof(Index));
            }

            var inquilinos = _repoInquilino.Listar()
                .Select(i => new { i.IdInquilino, NombreCompleto = i.Nombre + " " + i.Apellido })
                .ToList();

            var inmuebles = _repoInmueble.Listar();

            ViewBag.Inquilinos = new SelectList(inquilinos, "IdInquilino", "NombreCompleto", contrato.InquilinoId);
            ViewBag.Inmuebles = new SelectList(inmuebles, "IdInmueble", "Direccion", contrato.InmuebleId);

            return View("Create", contrato);
        }

        // GET: Contrato/Edit
        public IActionResult Edit(int id)
        {
            var contrato = _repo.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            return View("Edit", contrato);
        }

        // POST: Contrato/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contrato contrato)
        {
            if (id != contrato.IdContrato) return BadRequest();

            if (contrato.FechaInicio >= contrato.FechaFin)
            {
                ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");
            }

            if (_repo.ExisteOcupacion(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin, contrato.IdContrato))
            {
                ModelState.AddModelError("", "El inmueble ya está ocupado en ese rango de fechas.");
            }

            if (ModelState.IsValid)
            {
                _repo.Modificar(contrato);
                return RedirectToAction(nameof(Index));
            }

            var inquilinos = _repoInquilino.Listar()
                .Select(i => new { i.IdInquilino, NombreCompleto = i.Nombre + " " + i.Apellido })
                .ToList();

            var inmuebles = _repoInmueble.Listar();

            ViewBag.Inquilinos = new SelectList(inquilinos, "IdInquilino", "NombreCompleto", contrato.InquilinoId);
            ViewBag.Inmuebles = new SelectList(inmuebles, "IdInmueble", "Direccion", contrato.InmuebleId);

            return View("Edit", contrato);
        }

        // GET: Contrato/Delete
        public IActionResult Delete(int id)
        {
            var contrato = _repo.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            return View("Delete", contrato);
        }

        // POST: Contrato/Delete
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Contrato/Renovar
        public IActionResult Renovar(int id)
        {
            if (!PuedeRenovarContrato(id))
            {
                return BadRequest("El contrato no puede renovarse.");
            }

            var contratoViejo = _repo.ObtenerPorId(id);
            if (contratoViejo == null) return NotFound();

            // Crear contrato nuevo en base al viejo
            var contratoNuevo = new Contrato
            {
                InquilinoId = contratoViejo.InquilinoId,
                InmuebleId = contratoViejo.InmuebleId,
                FechaInicio = contratoViejo.FechaFin.AddDays(1), // empieza al día siguiente de que terminó
                FechaFin = contratoViejo.FechaFin.AddMonths(6),  // ejemplo: 6 meses
                Monto = contratoViejo.Monto
            };

            // Cargar listas para selects
            var inquilinos = _repoInquilino.Listar()
                .Select(i => new { i.IdInquilino, NombreCompleto = i.Nombre + " " + i.Apellido })
                .ToList();

            var inmuebles = _repoInmueble.Listar();

            ViewBag.Inquilinos = new SelectList(inquilinos, "IdInquilino", "NombreCompleto", contratoNuevo.InquilinoId);
            ViewBag.Inmuebles = new SelectList(inmuebles, "IdInmueble", "Direccion", contratoNuevo.InmuebleId);

            // Reusar la vista Create
            return View("Create", contratoNuevo);
        }

        
        private bool PuedeRenovarContrato(int contratoId)
        {
            var contrato = _repo.ObtenerPorId(contratoId);
            if (contrato == null) return false;

            var pagos = _repoPago.ObtenerPorContrato(contratoId);
            if (pagos == null || pagos.Count == 0) return false;

            // Renovable si todos los pagos fueron realizados y no anulados
            return pagos.All(p => p.Pagado && !p.Anulado);
        }
    }
}
