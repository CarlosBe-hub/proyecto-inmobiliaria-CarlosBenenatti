using ProyectoInmobiliaria.Models;

namespace ProyectoInmobiliaria.Repository
{
    public interface IContratoRepository
    {
        int Alta(Contrato contrato);
        int Baja(int idContrato);
        int Modificar(Contrato contrato);
        Contrato? ObtenerPorId(int idContrato);
        IList<Contrato> Listar();
    }
}
