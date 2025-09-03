using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ProyectoInmobiliaria.Models;

namespace ProyectoInmobiliaria.Repository
{
    public class InquilinoRepository : IInquilinoRepository
    {
        private readonly string _connectionString;

        public InquilinoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Alta(Inquilino inquilino)
        {
            int id = -1;
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"INSERT INTO inquilino
                (Nombre, Apellido, Documento, Telefono, Email, Direccion, Baja)
                VALUES (@nombre,@apellido,@documento,@telefono,@email,@direccion,@baja);";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@nombre", inquilino.Nombre);
            cmd.Parameters.AddWithValue("@apellido", inquilino.Apellido);
            cmd.Parameters.AddWithValue("@documento", inquilino.Documento);
            cmd.Parameters.AddWithValue("@telefono", (object?)inquilino.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)inquilino.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object?)inquilino.Direccion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@baja", inquilino.Baja);

            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;

            return id;
        }

        public int Baja(int idInquilino)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"DELETE FROM inquilino WHERE IdInquilino=@id;";
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", idInquilino);

            return cmd.ExecuteNonQuery();
        }

        public int Modificar(Inquilino inquilino)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"UPDATE inquilino SET
                Nombre=@nombre, Apellido=@apellido, Documento=@documento,
                Telefono=@telefono, Email=@email, Direccion=@direccion, Baja=@baja
                WHERE IdInquilino=@id;";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@nombre", inquilino.Nombre);
            cmd.Parameters.AddWithValue("@apellido", inquilino.Apellido);
            cmd.Parameters.AddWithValue("@documento", inquilino.Documento);
            cmd.Parameters.AddWithValue("@telefono", (object?)inquilino.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)inquilino.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object?)inquilino.Direccion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@baja", inquilino.Baja);
            cmd.Parameters.AddWithValue("@id", inquilino.IdInquilino);

            return cmd.ExecuteNonQuery();
        }

        public Inquilino? ObtenerPorId(int idInquilino)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"SELECT * FROM inquilino WHERE IdInquilino=@id;";
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", idInquilino);

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Inquilino
            {
                IdInquilino = rd.GetInt32("IdInquilino"),
                Nombre      = rd.GetString("Nombre"),
                Apellido    = rd.GetString("Apellido"),
                Documento   = rd.GetString("Documento"),
                Telefono    = rd.IsDBNull(rd.GetOrdinal("Telefono")) ? null : rd.GetString("Telefono"),
                Email       = rd.IsDBNull(rd.GetOrdinal("Email"))    ? null : rd.GetString("Email"),
                Direccion   = rd.IsDBNull(rd.GetOrdinal("Direccion"))? null : rd.GetString("Direccion"),
                Baja        = rd.GetBoolean("Baja")
            };
        }

        public IList<Inquilino> Listar(string? filtro = null, bool? soloActivos = null)
        {
            var lista = new List<Inquilino>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var sql = "SELECT * FROM inquilino WHERE 1=1";
            if (!string.IsNullOrEmpty(filtro))
                sql += " AND (Nombre LIKE @f OR Apellido LIKE @f OR Documento LIKE @f)";
            if (soloActivos.HasValue)
                sql += " AND Baja = @baja";

            sql += " ORDER BY Apellido,Nombre;";

            using var cmd = new MySqlCommand(sql, connection);
            if (!string.IsNullOrEmpty(filtro))
                cmd.Parameters.AddWithValue("@f", $"%{filtro}%");
            if (soloActivos.HasValue)
                cmd.Parameters.AddWithValue("@baja", !soloActivos.Value ? 1 : 0);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Inquilino
                {
                    IdInquilino = rd.GetInt32("IdInquilino"),
                    Nombre      = rd.GetString("Nombre"),
                    Apellido    = rd.GetString("Apellido"),
                    Documento   = rd.GetString("Documento"),
                    Telefono    = rd.IsDBNull(rd.GetOrdinal("Telefono")) ? null : rd.GetString("Telefono"),
                    Email       = rd.IsDBNull(rd.GetOrdinal("Email"))    ? null : rd.GetString("Email"),
                    Direccion   = rd.IsDBNull(rd.GetOrdinal("Direccion"))? null : rd.GetString("Direccion"),
                    Baja        = rd.GetBoolean("Baja")
                });
            }

            return lista;
        }
    }
}
