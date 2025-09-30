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

            ViewBag.Contrato = contrato;
            var pagos = _repoPago.ObtenerPorContrato(idContrato);
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
                Pagado = true
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
                _repoPago.Alta(pago);

                // Recalcular estado del contrato
                var pagos = _repoPago.ObtenerPorContrato(pago.IdContrato);
                var pagosRealizados = pagos.Count(p => p.Pagado && !p.Anulado);

                var contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
                if (contrato != null)
                {
                    contrato.Estado = pagosRealizados >= 6 ? "Finalizado" : "Activo";
                    _repoContrato.Modificar(contrato);
                }

                return RedirectToAction(nameof(Index), new { idContrato = pago.IdContrato });
            }

            return View(pago);
        }

        // GET: Pago/Edit/5
        public IActionResult Edit(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

            return View(pago);
        }

        // POST: Pago/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pago pago)
        {
            if (id != pago.IdPago) return NotFound();

            if (ModelState.IsValid)
            {
                _repoPago.Modificar(pago);

                // Recalcular estado del contrato
                var pagos = _repoPago.ObtenerPorContrato(pago.IdContrato);
                var pagosRealizados = pagos.Count(p => p.Pagado && !p.Anulado);

                var contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
                if (contrato != null)
                {
                    contrato.Estado = pagosRealizados >= 6 ? "Finalizado" : "Activo";
                    _repoContrato.Modificar(contrato);
                }

                return RedirectToAction(nameof(Index), new { idContrato = pago.IdContrato });
            }

            return View(pago);
        }

        // GET: Pago/Delete/5
        public IActionResult Delete(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();

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

                // Recalcular estado del contrato
                var pagos = _repoPago.ObtenerPorContrato(pago.IdContrato);
                var pagosRealizados = pagos.Count(p => p.Pagado && !p.Anulado);

                var contrato = _repoContrato.ObtenerPorId(pago.IdContrato);
                if (contrato != null)
                {
                    contrato.Estado = pagosRealizados >= 6 ? "Finalizado" : "Activo";
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
    }
}
