using ProyectoInmobiliaria.Models;

namespace ProyectoInmobiliaria.Repository
{
    public interface IPropietarioRepository
    {
        int Alta(Propietario propietario);
        int Baja(int idPropietario);
        int Modificar(Propietario propietario);
        Propietario? ObtenerPorId(int idPropietario);
        IList<Propietario> Listar(string? filtro = null, bool? soloActivos = null);
    }
}