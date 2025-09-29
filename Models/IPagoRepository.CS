using ProyectoInmobiliaria.Models;

namespace ProyectoInmobiliaria.Repository
{
    public interface IPagoRepository
    {
        int Alta(Pago pago);
        int Baja(int idPago);
        int Modificar(Pago pago);
        Pago? ObtenerPorId(int idPago);
        IList<Pago> ObtenerPorContrato(int idContrato);
        int ObtenerUltimoNumeroPago(int idContrato);
    }
}
