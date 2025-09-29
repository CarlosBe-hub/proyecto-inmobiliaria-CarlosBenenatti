using Microsoft.AspNetCore.Mvc;
using ProyectoInmobiliaria.Models;
using ProyectoInmobiliaria.Repository;

namespace ProyectoInmobiliaria.Controllers
{
    public class PagoController : Controller
    {
        private readonly IPagoRepository pagoRepo;
        private readonly IContratoRepository contratoRepo; 

        public PagoController(IPagoRepository pagoRepo, IContratoRepository contratoRepo) 
        {
            this.pagoRepo = pagoRepo;
            this.contratoRepo = contratoRepo;
        }

        // Lista todos los pagos de un contrato
        public IActionResult Index(int idContrato)
        {
            var lista = pagoRepo.ObtenerPorContrato(idContrato);
            ViewBag.IdContrato = idContrato;
            return View(lista);
        }

        // GET: Crear Pago
        public IActionResult Create(int idContrato)
        {
            var contrato = contratoRepo.ObtenerPorId(idContrato); 
            if (contrato == null)
            {
                return NotFound();
            }

            var nuevo = new Pago
            {
                IdContrato = idContrato,
                FechaPago = DateTime.Now,
                NumeroPago = pagoRepo.ObtenerUltimoNumeroPago(idContrato) + 1,
                Monto = contrato.Monto // <<--- autocompleta el monto
            };
            return View(nuevo);
        }

        // POST: Crear Pago
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            if (ModelState.IsValid)
            {
                pagoRepo.Alta(pago);
                return RedirectToAction(nameof(Index), new { idContrato = pago.IdContrato });
            }
            return View(pago);
        }

        // GET: Editar Pago
        public IActionResult Edit(int id)
        {
            var pago = pagoRepo.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        // POST: Editar Pago
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Pago pago)
        {
            if (ModelState.IsValid)
            {
                pagoRepo.Modificar(pago);
                return RedirectToAction(nameof(Index), new { idContrato = pago.IdContrato });
            }
            return View(pago);
        }

        // Anular pago (soft delete)
        public IActionResult Anular(int id)
        {
            var pago = pagoRepo.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }

            pagoRepo.Baja(id);
            return RedirectToAction(nameof(Index), new { idContrato = pago.IdContrato });
        }

        // GET: Detalle Pago
        public IActionResult Details(int id)
        {
            var pago = pagoRepo.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }
    }
}
