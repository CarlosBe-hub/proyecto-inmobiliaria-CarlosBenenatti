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

        public ContratoController(
            IContratoRepository repo,
            IInquilinoRepository repoInquilino,
            IInmuebleRepository repoInmueble)
        {
            _repo = repo;
            _repoInquilino = repoInquilino;
            _repoInmueble = repoInmueble;
        }

        // GET: Contrato
        public IActionResult Index(int pageNumber = 1, int pageSize = 5)
        {
            var (contratos, totalCount) = _repo.ListarPaginado(pageNumber, pageSize);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            return View(contratos);
        }

        // GET: Contrato/Detalle
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

            if (_repo.ExisteOcupacion(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin))
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

            // Excluir el contrato actual en la validación
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

        // GET: Contrato/Active
        public IActionResult Active(int pageNumber = 1, int pageSize = 5)
        {
            var lista = _repo.Listar()
                .Where(c => c.FechaInicio <= DateTime.Today && c.FechaFin >= DateTime.Today)
                .ToList();

            var totalCount = lista.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var contratosPaginados = lista.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            return View("Index", contratosPaginados);
        }

        // GET: Contrato/ByTenant
        public IActionResult ByTenant(int tenantId, int pageNumber = 1, int pageSize = 5)
        {
            var lista = _repo.Listar()
                .Where(c => c.InquilinoId == tenantId)
                .ToList();

            var totalCount = lista.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var contratosPaginados = lista.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            return View("Index", contratosPaginados);
        }

        // GET: Contrato/ByOwner
        public IActionResult ByOwner(int ownerId, int pageNumber = 1, int pageSize = 5)
        {
            var inmuebles = _repoInmueble.Listar()
                .Where(i => i.PropietarioId == ownerId)
                .Select(i => i.IdInmueble);

            var lista = _repo.Listar()
                .Where(c => inmuebles.Contains(c.InmuebleId))
                .ToList();

            var totalCount = lista.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var contratosPaginados = lista.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            return View("Index", contratosPaginados);
        }
    }
}
