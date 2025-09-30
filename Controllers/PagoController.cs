using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria.Models;
using ProyectoInmobiliaria.Repository;
using System.Linq;

namespace ProyectoInmobiliaria.Controllers
{
    public class PagoController : Controller
    {
        private readonly IPagoRepository _repoPago;
        private readonly IContratoRepository _repoContrato;

        public PagoController(IPagoRepository repoPago, IContratoRepository repoContrato)
        {
            _repoPago = repoPago;
            _repoContrato = repoContrato;
        }

        // GET: Pago/Index/5
        public IActionResult Index(int idContrato)
        {
            var pagos = _repoPago.ObtenerPorContrato(idContrato);
            var contrato = _repoContrato.ObtenerPorId(idContrato);

            if (contrato == null) return NotFound();

            // Calcular meses del contrato
            var totalMeses = ((contrato.FechaFin.Year - contrato.FechaInicio.Year) * 12) +
                             (contrato.FechaFin.Month - contrato.FechaInicio.Month);

            // Asegurar mínimo 6 pagos
            if (totalMeses < 6)
                totalMeses = 6;

            var pagosRealizados = pagos.Count(p => p.Pagado && !p.Anulado);
            var pagosFaltantes = totalMeses - pagosRealizados;

            ViewBag.Contrato = contrato;
            ViewBag.TotalEsperado = totalMeses;
            ViewBag.PagosRealizados = pagosRealizados;
            ViewBag.PagosFaltantes = pagosFaltantes;

            return View(pagos);
        }

        // GET: Pago/Create
        public IActionResult Create(int idContrato)
        {
            var contrato = _repoContrato.ObtenerPorId(idContrato);
            if (contrato == null) return NotFound();

            var numeroPago = _repoPago.ObtenerUltimoNumeroPago(idContrato) + 1;

            var pago = new Pago
            {
                IdContrato = idContrato,
                NumeroPago = numeroPago,
                FechaPago = DateTime.Today,
                Monto = contrato.Monto,
                Pagado = true,   // <-- ya lo dejamos marcado
                Anulado = false  // <-- nunca nulo por defecto
            };

            return View(pago);
        }

        // POST: Pago/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                pago.Pagado = true;   // <-- forzamos a pagado
                pago.Anulado = false; // <-- siempre no anulado

                _repoPago.Alta(pago);
                return RedirectToAction(nameof(Index), new { idContrato = pago.IdContrato });
            }

            return View(pago);
        }

        // GET: Pago/Edit
        public IActionResult Edit(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            return View(pago);
        }

        // POST: Pago/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pago pago)
        {
            if (id != pago.IdPago) return BadRequest();

            if (ModelState.IsValid)
            {
                pago.Pagado = true;   // <-- siempre mantener pagado
                pago.Anulado = false; // <-- y no anulado salvo acción especial

                _repoPago.Modificar(pago);
                return RedirectToAction(nameof(Index), new { idContrato = pago.IdContrato });
            }

            return View(pago);
        }

        // GET: Pago/Delete
        public IActionResult Delete(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            return View(pago);
        }

        // POST: Pago/DeleteConfirmed
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            _repoPago.Baja(id);
            return RedirectToAction(nameof(Index), new { idContrato = pago.IdContrato });
        }
    }
}


