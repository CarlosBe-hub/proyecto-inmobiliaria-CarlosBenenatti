using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ProyectoInmobiliaria.Models;

namespace ProyectoInmobiliaria.Repository
{
    public class PropietarioRepository : IPropietarioRepository
    {
        private readonly string _connectionString;

        public PropietarioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Alta(Propietario propietario)
        {
            int id = -1;
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"INSERT INTO propietario
                (nombre, apellido, documento, telefono, email, direccion, baja)
                VALUES (@nombre,@apellido,@documento,@telefono,@email,@direccion,@baja);";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@nombre", propietario.Nombre);
            cmd.Parameters.AddWithValue("@apellido", propietario.Apellido);
            cmd.Parameters.AddWithValue("@documento", propietario.Documento);
            cmd.Parameters.AddWithValue("@telefono", (object?)propietario.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)propietario.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object?)propietario.Direccion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@baja", propietario.Baja);

            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;

            return id;
        }

        public int Baja(int idPropietario)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"DELETE FROM propietario WHERE id_propietario=@id;";
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", idPropietario);

            return cmd.ExecuteNonQuery();
        }

        public int Modificar(Propietario propietario)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"UPDATE propietario SET
                nombre=@nombre, apellido=@apellido, documento=@documento,
                telefono=@telefono, email=@email, direccion=@direccion, baja=@baja
                WHERE id_propietario=@id;";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@nombre", propietario.Nombre);
            cmd.Parameters.AddWithValue("@apellido", propietario.Apellido);
            cmd.Parameters.AddWithValue("@documento", propietario.Documento);
            cmd.Parameters.AddWithValue("@telefono", (object?)propietario.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)propietario.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object?)propietario.Direccion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@baja", propietario.Baja);
            cmd.Parameters.AddWithValue("@id", propietario.IdPropietario);

            return cmd.ExecuteNonQuery();
        }

        public Propietario? ObtenerPorId(int idPropietario)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"SELECT * FROM propietario WHERE id_propietario=@id;";
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", idPropietario);

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Propietario
            {
                IdPropietario = rd.GetInt32("id_propietario"),
                Nombre        = rd.GetString("nombre"),
                Apellido      = rd.GetString("apellido"),
                Documento     = rd.GetString("documento"),
                Telefono      = rd.IsDBNull(rd.GetOrdinal("telefono")) ? null : rd.GetString("telefono"),
                Email         = rd.IsDBNull(rd.GetOrdinal("email"))    ? null : rd.GetString("email"),
                Direccion     = rd.IsDBNull(rd.GetOrdinal("direccion"))? null : rd.GetString("direccion"),
                Baja          = rd.GetBoolean("baja")
            };
        }

        public IList<Propietario> Listar(string? filtro = null, bool? soloActivos = null)
        {
            var lista = new List<Propietario>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var sql = "SELECT * FROM propietario WHERE 1=1";
            if (!string.IsNullOrEmpty(filtro))
                sql += " AND (nombre LIKE @f OR apellido LIKE @f OR documento LIKE @f)";
            if (soloActivos.HasValue)
                sql += " AND baja = @baja";

            sql += " ORDER BY apellido,nombre;";

            using var cmd = new MySqlCommand(sql, connection);
            if (!string.IsNullOrEmpty(filtro))
                cmd.Parameters.AddWithValue("@f", $"%{filtro}%");
            if (soloActivos.HasValue)
                cmd.Parameters.AddWithValue("@baja", !soloActivos.Value ? 1 : 0);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Propietario
                {
                    IdPropietario = rd.GetInt32("id_propietario"),
                    Nombre        = rd.GetString("nombre"),
                    Apellido      = rd.GetString("apellido"),
                    Documento     = rd.GetString("documento"),
                    Telefono      = rd.IsDBNull(rd.GetOrdinal("telefono")) ? null : rd.GetString("telefono"),
                    Email         = rd.IsDBNull(rd.GetOrdinal("email"))    ? null : rd.GetString("email"),
                    Direccion     = rd.IsDBNull(rd.GetOrdinal("direccion"))? null : rd.GetString("direccion"),
                    Baja          = rd.GetBoolean("baja")
                });
            }

            return lista;
        }
    }
}
