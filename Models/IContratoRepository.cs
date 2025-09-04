using ProyectoInmobiliaria.Models;

namespace ProyectoInmobiliaria.Repository
{
    public interface IContratoRepository
    {
        void Alta(Contrato contrato);
        void Baja(int id);
        void Modificar(Contrato contrato);
        Contrato? ObtenerPorId(int id);
        IList<Contrato> Listar();
        bool ExisteOcupacion(int inmuebleId, DateTime fechaInicio, DateTime fechaFin, int? idContratoExcluir = null);
    }
}
