using ProyectoInmobiliaria.Models;  

namespace ProyectoInmobiliaria.Repository
{
    public interface IRepositorioUsuario
    {
        int Alta(Usuario usuario);
        void Modificar(Usuario usuario);
        void Eliminar(int id);
        Usuario? ObtenerPorId(int id);
        Usuario? ObtenerPorEmail(string email);
        List<Usuario> ObtenerTodos();
    }
}
