namespace ProyectoInmobiliaria.Models
{
    public class Inmueble
    {
        public int IdInmueble { get; set; }
        public int IdEstadoInmueble { get; set; }
        public int IdTipoInmueble { get; set; }
        public int IdPropietario { get; set; }

        public string Direccion { get; set; } = "";
        public int CantidadAmbientes { get; set; }
        public decimal SuperficieM2 { get; set; }
    }
}
