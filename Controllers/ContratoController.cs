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
                contrato.Estado = "Activo";
                _repo.Alta(contrato);
                TempData["Success"] = "Contrato creado correctamente.";
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
                TempData["Success"] = "Contrato modificado correctamente.";
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
            TempData["Success"] = "Contrato eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Contrato/Renovar
        public IActionResult Renovar(int id)
        {
            var contratoViejo = _repo.ObtenerPorId(id);
            if (contratoViejo == null) return NotFound();

            if (!PuedeRenovarContrato(id))
            {
                TempData["Error"] = "No se puede renovar este contrato porque ya finalizó o ya existe uno activo.";
                return RedirectToAction(nameof(Index));
            }

            contratoViejo.Estado = "Finalizado";
            _repo.Modificar(contratoViejo);

            var contratoNuevo = new Contrato
            {
                InquilinoId = contratoViejo.InquilinoId,
                InmuebleId = contratoViejo.InmuebleId,
                FechaInicio = contratoViejo.FechaFin.AddDays(1),
                FechaFin = contratoViejo.FechaFin.AddMonths(6),
                Monto = contratoViejo.Monto,
                Estado = "Activo"
            };

            var inquilinos = _repoInquilino.Listar()
                .Select(i => new { i.IdInquilino, NombreCompleto = i.Nombre + " " + i.Apellido })
                .ToList();

            var inmuebles = _repoInmueble.Listar();

            ViewBag.Inquilinos = new SelectList(inquilinos, "IdInquilino", "NombreCompleto", contratoNuevo.InquilinoId);
            ViewBag.Inmuebles = new SelectList(inmuebles, "IdInmueble", "Direccion", contratoNuevo.InmuebleId);

            return View("Create", contratoNuevo);
        }

        private bool PuedeRenovarContrato(int contratoId)
        {
            var contrato = _repo.ObtenerPorId(contratoId);
            if (contrato == null) return false;

            if (contrato.Estado == "Finalizado")
                return false;

            var contratosMismoInmueble = _repo.Listar()
                .Where(c => c.InmuebleId == contrato.InmuebleId && c.Estado == "Activo" && c.IdContrato != contrato.IdContrato)
                .ToList();

            if (contratosMismoInmueble.Any())
                return false;

            var pagos = _repoPago.ObtenerPorContrato(contratoId);
            if (pagos == null) return false;

            var pagosRealizados = pagos.Count(p => p.Pagado && !p.Anulado);

            return pagosRealizados >= 6;
        }
    }
}
