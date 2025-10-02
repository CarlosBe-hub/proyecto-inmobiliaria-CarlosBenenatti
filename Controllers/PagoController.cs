using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria.Models;
using ProyectoInmobiliaria.Repository;

namespace ProyectoInmobiliaria.Controllers
{
    public class PagoController : Controller
    {
        private readonly IContratoRepository _repoContrato;
        private readonly IPagoRepository _repoPago;

        public PagoController(IContratoRepository repoContrato, IPagoRepository repoPago)
        {
            _repoContrato = repoContrato;
            _repoPago = repoPago;
        }

        // GET: Pago
        public IActionResult Index(int idContrato)
        {
            var contrato = _repoContrato.ObtenerPorId(idContrato);
            if (contrato == null) return NotFound();

            var pagos = _repoPago.ObtenerPorContrato(idContrato);

            // Calcular totales
            var pagosRealizados = pagos.Count(p => p.Pagado && !p.Anulado);
            var totalEsperado = 6; // fijo en 6 pagos (esto se puede cambiar a futuro)
            var pagosFaltantes = totalEsperado - pagosRealizados;

            ViewBag.Contrato = contrato;
            ViewBag.TotalEsperado = totalEsperado;
            ViewBag.PagosRealizados = pagosRealizados;
            ViewBag.PagosFaltantes = pagosFaltantes;

            return View(pagos);
        }

        // GET: Pago/Details/5
        public IActionResult Details(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            return View(pago);
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
                Pagado = true,
                Anulado = false,
                Monto = contrato.Monto
            };

            ViewBag.Contrato = contrato;
            return View(pago);
        }

        // POST: Pago/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            var contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
            if (contrato == null) return NotFound();

            pago.Monto = contrato.Monto;

            if (ModelState.IsValid)
            {
                _repoPago.Alta(pago);

                var pagos = _repoPago.ObtenerPorContrato(pago.IdContrato);
                var pagosRealizados = pagos.Count(p => p.Pagado && !p.Anulado);

                if (pagosRealizados >= 6)
                {
                    _repoContrato.TerminarAnticipado(contrato.IdContrato, contrato.FechaFin, 0);
                    TempData["SuccessMessage"] = "Pago registrado correctamente. Se completaron 6 pagos y el contrato ha sido finalizado.";
                }
                else
                {
                    contrato.Estado = "Activo";
                    _repoContrato.Modificar(contrato);
                    TempData["SuccessMessage"] = "Pago registrado correctamente.";
                }

                return RedirectToAction(nameof(Index), new { idContrato = pago.IdContrato });
            }

            ViewBag.Contrato = contrato;
            return View(pago);
        }

        // GET: Pago/Edit/5
        public IActionResult Edit(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            var contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
            if (contrato == null) return NotFound();

            ViewBag.Contrato = contrato;
            return View(pago);
        }

        // POST: Pago/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pago pago)
        {
            if (id != pago.IdPago) return NotFound();

            var contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
            if (contrato == null) return NotFound();

            pago.Monto = contrato.Monto;

            if (ModelState.IsValid)
            {
                _repoPago.Modificar(pago);

                var pagos = _repoPago.ObtenerPorContrato(pago.IdContrato);
                var pagosRealizados = pagos.Count(p => p.Pagado && !p.Anulado);

                if (pagosRealizados >= 6)
                    _repoContrato.TerminarAnticipado(contrato.IdContrato, contrato.FechaFin, 0);
                else
                {
                    contrato.Estado = "Activo";
                    _repoContrato.Modificar(contrato);
                }

                TempData["SuccessMessage"] = "Pago editado correctamente.";
                return RedirectToAction(nameof(Index), new { idContrato = pago.IdContrato });
            }

            ViewBag.Contrato = contrato;
            return View(pago);
        }

        // GET: Pago/Delete/5
        public IActionResult Delete(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            var contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
            ViewBag.Contrato = contrato;

            return View(pago);
        }

        // POST: Pago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null)
            {
                TempData["ErrorMessage"] = "El pago no existe.";
                return RedirectToAction(nameof(Index), new { idContrato = 0 });
            }

            try
            {
                var result = _repoPago.Baja(id);
                if (result <= 0)
                {
                    TempData["ErrorMessage"] = "No se pudo anular el pago.";
                    return RedirectToAction(nameof(Index), new { idContrato = pago.IdContrato });
                }

                var pagos = _repoPago.ObtenerPorContrato(pago.IdContrato);
                var pagosRealizados = pagos.Count(p => p.Pagado && !p.Anulado);

                var contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
                if (contrato != null)
                {
                    if (pagosRealizados < 6 && contrato.Estado == "Finalizado")
                        _repoContrato.ReactivarContrato(contrato.IdContrato);
                    else
                        contrato.Estado = "Activo";

                    _repoContrato.Modificar(contrato);
                }

                TempData["SuccessMessage"] = "El pago fue anulado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un error al intentar anular el pago: " + ex.Message;
            }

            return RedirectToAction(nameof(Index), new { idContrato = pago.IdContrato });
        }

        // GET: Pago/FinalizarAnticipado/5
        public IActionResult FinalizarAnticipado(int idContrato)
        {
            var contrato = _repoContrato.ObtenerPorId(idContrato);
            if (contrato == null) return NotFound();

            var pagos = _repoPago.ObtenerPorContrato(idContrato)?.Count(p => p.Pagado && !p.Anulado) ?? 0;
            if (pagos < 1)
            {
                TempData["ErrorMessage"] = "No se puede finalizar anticipadamente sin al menos un pago registrado.";
                return RedirectToAction(nameof(Index), new { idContrato });
            }

            ViewBag.Contrato = contrato;
            return View(contrato);
        }

        // POST: Pago/FinalizarAnticipado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinalizarAnticipado(int idContrato, DateTime fechaAnticipada)
        {
            var contrato = _repoContrato.ObtenerPorId(idContrato);
            if (contrato == null) return NotFound();

            var pagos = _repoPago.ObtenerPorContrato(idContrato)?.Count(p => p.Pagado && !p.Anulado) ?? 0;
            if (pagos < 1)
            {
                TempData["ErrorMessage"] = "No se puede finalizar anticipadamente sin al menos un pago registrado.";
                return RedirectToAction(nameof(Index), new { idContrato });
            }

            if (fechaAnticipada < contrato.FechaInicio)
            {
                TempData["ErrorMessage"] = "La fecha de finalización anticipada no puede ser anterior a la fecha de inicio del contrato.";
                return RedirectToAction(nameof(Index), new { idContrato });
            }

            var totalDias = (contrato.FechaFin - contrato.FechaInicio).TotalDays;
            var diasCumplidos = (fechaAnticipada - contrato.FechaInicio).TotalDays;

            decimal multa = diasCumplidos < totalDias / 2 ? contrato.Monto * 2 : contrato.Monto;

            _repoContrato.TerminarAnticipado(contrato.IdContrato, fechaAnticipada, multa);

            TempData["SuccessMessage"] = $"Contrato finalizado anticipadamente. Multa aplicada: {multa:C}";
            return RedirectToAction(nameof(Index), new { idContrato });
        }
    }
}
