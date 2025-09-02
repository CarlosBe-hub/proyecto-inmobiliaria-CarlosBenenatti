using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ProyectoInmobiliaria.Models;

namespace ProyectoInmobiliaria.Repository
{
    public class InmuebleRepository : IInmuebleRepository
    {
        private readonly string _connectionString;

        public InmuebleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Alta(Inmueble inmueble)
        {
            int id = -1;
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"INSERT INTO inmueble
                (direccion, cantidad_ambientes, superficie, tipo, estado, id_propietario)
                VALUES (@direccion,@cantidad_ambientes,@superficie,@tipo,@estado,@id_propietario);";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@direccion", inmueble.Direccion);
            cmd.Parameters.AddWithValue("@cantidad_ambientes", inmueble.CantidadAmbientes);
            cmd.Parameters.AddWithValue("@superficie", inmueble.SuperficieM2);
            cmd.Parameters.AddWithValue("@tipo", inmueble.IdTipoInmueble);
            cmd.Parameters.AddWithValue("@estado", inmueble.IdEstadoInmueble);
            cmd.Parameters.AddWithValue("@id_propietario", inmueble.IdPropietario);

            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;

            return id;
        }

        public int Baja(int idInmueble)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"DELETE FROM inmueble WHERE id_inmueble=@id;";
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", idInmueble);

            return cmd.ExecuteNonQuery();
        }

        public int Modificar(Inmueble inmueble)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"UPDATE inmueble SET
                direccion=@direccion, cantidad_ambientes=@cantidad_ambientes,
                superficie=@superficie, tipo=@tipo, estado=@estado, id_propietario=@id_propietario
                WHERE id_inmueble=@id;";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@direccion", inmueble.Direccion);
            cmd.Parameters.AddWithValue("@cantidad_ambientes", inmueble.CantidadAmbientes);
            cmd.Parameters.AddWithValue("@superficie", inmueble.SuperficieM2);
            cmd.Parameters.AddWithValue("@tipo", inmueble.IdTipoInmueble);
            cmd.Parameters.AddWithValue("@estado", inmueble.IdEstadoInmueble);
            cmd.Parameters.AddWithValue("@id_propietario", inmueble.IdPropietario);
            cmd.Parameters.AddWithValue("@id", inmueble.IdInmueble);

            return cmd.ExecuteNonQuery();
        }

        public Inmueble? ObtenerPorId(int idInmueble)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"SELECT * FROM inmueble WHERE id_inmueble=@id;";
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", idInmueble);

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Inmueble
            {
                IdInmueble       = rd.GetInt32("id_inmueble"),
                Direccion        = rd.GetString("direccion"),
                CantidadAmbientes= rd.GetInt32("cantidad_ambientes"),
                SuperficieM2     = rd.GetDecimal("superficie"),
                IdTipoInmueble   = rd.GetInt32("tipo"),
                IdEstadoInmueble = rd.GetInt32("estado"),
                IdPropietario    = rd.GetInt32("id_propietario")
            };
        }

        public IList<Inmueble> Listar()
        {
            var lista = new List<Inmueble>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"SELECT * FROM inmueble ORDER BY direccion;";
            using var cmd = new MySqlCommand(sql, connection);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Inmueble
                {
                    IdInmueble       = rd.GetInt32("id_inmueble"),
                    Direccion        = rd.GetString("direccion"),
                    CantidadAmbientes= rd.GetInt32("cantidad_ambientes"),
                    SuperficieM2     = rd.GetDecimal("superficie"),
                    IdTipoInmueble   = rd.GetInt32("tipo"),
                    IdEstadoInmueble = rd.GetInt32("estado"),
                    IdPropietario    = rd.GetInt32("id_propietario")
                });
            }

            return lista;
        }
    }
}
