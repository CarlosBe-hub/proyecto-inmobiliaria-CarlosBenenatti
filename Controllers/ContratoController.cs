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
        public IActionResult Index(DateTime? fechaDesde, DateTime? fechaHasta, int pageNumber = 1, int pageSize = 5)
        {
            IEnumerable<Contrato> contratos;
            int totalCount;

            bool isFiltering = fechaDesde.HasValue && fechaHasta.HasValue;

            if (isFiltering)
            {
                contratos = _repo.ListarVigentes(fechaDesde.Value, fechaHasta.Value);
                totalCount = contratos.Count();
            }
            else
            {
                var result = _repo.ListarPaginado(pageNumber, pageSize);
                contratos = result.Contratos;
                totalCount = result.TotalCount;
            }

            var renovables = contratos.ToDictionary(
                c => c.IdContrato,
                c => PuedeRenovarContrato(c.IdContrato)
            );

            var pagosActivos = contratos.ToDictionary(
                c => c.IdContrato,
                c => _repoPago.ObtenerPorContrato(c.IdContrato)
                              .Count(p => p.Pagado && !p.Anulado)
            );

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.Renovables = renovables;
            ViewBag.PagosActivos = pagosActivos;


            ViewBag.IsFiltering = isFiltering;
            ViewBag.FilterFechaDesde = fechaDesde?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.FilterFechaHasta = fechaHasta?.ToString("yyyy-MM-dd") ?? "";

            return View(contratos);
        }

        // GET: Contrato/Vigentes
        public IActionResult Vigentes()
        {
            var hoy = DateTime.Today;
            var contratosVigentes = _repo.Listar()
                                         .Where(c => c.Estado == "Activo"
                                                  && c.FechaInicio <= hoy
                                                  && c.FechaFin >= hoy)
                                         .ToList();

            return View("Vigentes", contratosVigentes);
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
                ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");

            if (_repo.ExisteOcupacion(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin, contrato.IdContrato))
                ModelState.AddModelError("", "El inmueble ya está ocupado en ese rango de fechas.");

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
                ModelState.AddModelError("", "La fecha de inicio debe ser anterior a la fecha de fin.");

            if (_repo.ExisteOcupacion(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin, contrato.IdContrato))
                ModelState.AddModelError("", "El inmueble ya está ocupado en ese rango de fechas.");

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
                TempData["Error"] = "No se puede renovar este contrato.";
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
            if (contrato.Estado == "Finalizado") return false;

            var contratosMismoInmueble = _repo.Listar()
                                             .Where(c => c.InmuebleId == contrato.InmuebleId && c.Estado == "Activo" && c.IdContrato != contrato.IdContrato)
                                             .ToList();

            if (contratosMismoInmueble.Any()) return false;

            var pagos = _repoPago.ObtenerPorContrato(contratoId);
            if (pagos == null) return false;

            var pagosRealizados = pagos.Count(p => p.Pagado && !p.Anulado);
            return pagosRealizados >= 6;
        }

        // POST: Contrato/FinalizarAnticipado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinalizarAnticipado(int id, DateTime fechaAnticipada)
        {
            var contrato = _repo.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            if (fechaAnticipada < contrato.FechaInicio || fechaAnticipada > contrato.FechaFin)
            {
                TempData["Error"] = "La fecha anticipada debe estar dentro del rango del contrato.";
                return RedirectToAction(nameof(Details), new { id = contrato.IdContrato });
            }

            // Calcular multa según si cumplió menos de la mitad del contrato
            double totalDias = (contrato.FechaFin - contrato.FechaInicio).TotalDays;
            double diasCumplidos = (fechaAnticipada - contrato.FechaInicio).TotalDays;
            decimal multa = diasCumplidos < totalDias / 2 ? contrato.Monto * 2 : contrato.Monto;

            contrato.FechaAnticipada = fechaAnticipada;
            contrato.Multa = multa;
            contrato.Estado = "Finalizado";
            _repo.Modificar(contrato);

            // Generar pago por la multa
            var pagosExistentes = _repoPago.ObtenerPorContrato(contrato.IdContrato);
            int nroPago = pagosExistentes?.Any() == true ? pagosExistentes.Max(p => p.NumeroPago) + 1 : 1;

            var pagoMulta = new Pago
            {
                IdContrato = contrato.IdContrato,
                NumeroPago = nroPago,
                FechaPago = DateTime.Now,
                Monto = multa,
                Detalle = $"Multa por finalización anticipada ({fechaAnticipada:dd/MM/yyyy})",
                MetodoPago = "Pendiente",
                Pagado = false,
                Anulado = false
            };

            _repoPago.Alta(pagoMulta);

            TempData["Success"] = $"Contrato finalizado anticipadamente. Multa generada: {multa:C}";
            return RedirectToAction(nameof(Details), new { id = contrato.IdContrato });
        }
    }
}
