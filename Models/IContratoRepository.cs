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

        // Método para validar contratos asociados a un inmueble
        IList<Contrato> BuscarPorInmueble(int inmuebleId);

        (IList<Contrato> Contratos, int TotalCount) ListarPaginado(int pageNumber, int pageSize);

        IEnumerable<Contrato> ListarVigentes(DateTime fechaDesde, DateTime fechaHasta);

        void TerminarAnticipado(int contratoId, DateTime fechaAnticipada, decimal multa);
        void ReactivarContrato(int contratoId);
    }
}