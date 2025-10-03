using MySql.Data.MySqlClient;
using ProyectoInmobiliaria.Models;
using System.Collections.Generic;

namespace ProyectoInmobiliaria.Repository
{
    public class RepositorioUsuario : IRepositorioUsuario
    {
        private readonly string _connectionString;

        public RepositorioUsuario(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Alta(Usuario usuario)
        {
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"INSERT INTO Usuarios (Nombre, Apellido, Email, ClaveHash, Rol, Avatar) 
                                 VALUES (@Nombre, @Apellido, @Email, @ClaveHash, @Rol, @Avatar); 
                                 SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
            cmd.Parameters.AddWithValue("@Email", usuario.Email);
            cmd.Parameters.AddWithValue("@ClaveHash", usuario.ClaveHash);
            cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
            cmd.Parameters.AddWithValue("@Avatar", usuario.Avatar ?? (object)DBNull.Value);
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Modificar(Usuario usuario)
        {
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"UPDATE Usuarios SET Nombre = @Nombre, Apellido = @Apellido, Email = @Email, 
                                 ClaveHash = @ClaveHash, Rol = @Rol, Avatar = @Avatar WHERE IdUsuario = @IdUsuario";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
            cmd.Parameters.AddWithValue("@Email", usuario.Email);
            cmd.Parameters.AddWithValue("@ClaveHash", usuario.ClaveHash);
            cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
            cmd.Parameters.AddWithValue("@Avatar", usuario.Avatar ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);  // Cambiado de usuario.Id a usuario.IdUsuario
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"DELETE FROM Usuarios WHERE IdUsuario = @IdUsuario";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdUsuario", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public Usuario? ObtenerPorId(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"SELECT * FROM Usuarios WHERE IdUsuario = @IdUsuario";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdUsuario", id);
            conn.Open();

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    IdUsuario = reader.GetInt32("IdUsuario"),  // Cambiado a IdUsuario
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Email = reader.GetString("Email"),
                    ClaveHash = reader.GetString("ClaveHash"),
                    Rol = reader.GetString("Rol"),
                    Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar")
                };
            }
            return null;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"SELECT * FROM Usuarios WHERE Email = @Email";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            conn.Open();

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    IdUsuario = reader.GetInt32("IdUsuario"),  // Cambiado a IdUsuario
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Email = reader.GetString("Email"),
                    ClaveHash = reader.GetString("ClaveHash"),
                    Rol = reader.GetString("Rol"),
                    Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar")
                };
            }
            return null;
        }

        public List<Usuario> ObtenerTodos()
        {
            var usuarios = new List<Usuario>();
            using var conn = new MySqlConnection(_connectionString);
            const string sql = @"SELECT * FROM Usuarios";
            using var cmd = new MySqlCommand(sql, conn);
            conn.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                usuarios.Add(new Usuario
                {
                    IdUsuario = reader.GetInt32("IdUsuario"),  // Cambiado a IdUsuario
                    Nombre = reader.GetString("Nombre"),
                    Apellido = reader.GetString("Apellido"),
                    Email = reader.GetString("Email"),
                    ClaveHash = reader.GetString("ClaveHash"),
                    Rol = reader.GetString("Rol"),
                    Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar")
                });
            }
            return usuarios;
        }
    }
}
