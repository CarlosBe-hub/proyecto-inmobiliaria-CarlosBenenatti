using System;

namespace ProyectoInmobiliaria.Models
{
    public class Pago
    {
        public int IdPago { get; set; }

        public int IdContrato { get; set; }

        public string? MetodoPago { get; set; } // Tarjeta / Efectivo

        public DateTime FechaPago { get; set; }

        public decimal Monto { get; set; }

        public string? Detalle { get; set; }

        public bool Anulado { get; set; }

        public int NumeroPago { get; set; }

        public Contrato? Contrato { get; set; }

        public bool Pagado { get; set; }
    }
}
