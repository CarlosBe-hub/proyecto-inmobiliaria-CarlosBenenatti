using MySql.Data.MySqlClient;

namespace ProyectoInmobiliaria.Models
{
    public abstract class RepositorioBase
    {
        protected readonly string? connectionString;

        protected RepositorioBase(IConfiguration configuration)
        {
            
            connectionString = configuration.GetConnectionString("Inmobiliaria");
        }

        protected MySqlConnection GetConnection()
        {
            var connection = new MySqlConnection(connectionString);
            connection.Open(); 
            return connection;
        }
    }
}