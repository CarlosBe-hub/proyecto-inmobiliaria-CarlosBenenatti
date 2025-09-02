using ProyectoInmobiliaria.Models;

namespace ProyectoInmobiliaria.Repository
{
    public interface IInmuebleRepository
    {
        int Alta(Inmueble inmueble);
        int Baja(int idInmueble);
        int Modificar(Inmueble inmueble);
        Inmueble? ObtenerPorId(int idInmueble);
        IList<Inmueble> Listar();
    }
}