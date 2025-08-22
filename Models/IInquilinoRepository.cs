using ProyectoInmobiliaria.Models;

namespace ProyectoInmobiliaria.Repository
{
    public interface IInquilinoRepository
    {
        int Alta(Inquilino inquilino);
        int Baja(int idInquilino);
        int Modificar(Inquilino inquilino);
        Inquilino? ObtenerPorId(int idInquilino);
        IList<Inquilino> Listar(string? filtro = null, bool? soloActivos = null);
    }
}