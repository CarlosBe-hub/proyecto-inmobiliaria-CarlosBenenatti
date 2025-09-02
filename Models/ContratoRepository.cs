using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ProyectoInmobiliaria.Models;

namespace ProyectoInmobiliaria.Repository
{
    public class ContratoRepository : IContratoRepository
    {
        private readonly string _connectionString;

        public ContratoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Alta(Contrato contrato)
        {
            int id = -1;
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"INSERT INTO contrato
                (monto, fecha_desde, fecha_hasta, id_inquilino, id_inmueble)
                VALUES (@monto,@desde,@hasta,@inquilino,@inmueble);";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@monto", contrato.Monto);
            cmd.Parameters.AddWithValue("@desde", contrato.FechaDesde);
            cmd.Parameters.AddWithValue("@hasta", contrato.FechaHasta);
            cmd.Parameters.AddWithValue("@inquilino", contrato.IdInquilino);
            cmd.Parameters.AddWithValue("@inmueble", contrato.IdInmueble);

            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;
            return id;
        }

        public int Modificar(Contrato contrato)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"UPDATE contrato SET
                monto=@monto, fecha_desde=@desde, fecha_hasta=@hasta,
                id_inquilino=@inquilino, id_inmueble=@inmueble
                WHERE id_contrato=@id;";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@monto", contrato.Monto);
            cmd.Parameters.AddWithValue("@desde", contrato.FechaDesde);
            cmd.Parameters.AddWithValue("@hasta", contrato.FechaHasta);
            cmd.Parameters.AddWithValue("@inquilino", contrato.IdInquilino);
            cmd.Parameters.AddWithValue("@inmueble", contrato.IdInmueble);
            cmd.Parameters.AddWithValue("@id", contrato.IdContrato);

            return cmd.ExecuteNonQuery();
        }

        public int Baja(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = "DELETE FROM contrato WHERE id_contrato=@id;";
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);

            return cmd.ExecuteNonQuery();
        }

        public Contrato? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"
                SELECT c.id_contrato, c.monto, c.fecha_desde, c.fecha_hasta,
                       c.id_inquilino, i.nombre, i.apellido,
                       c.id_inmueble, im.direccion AS direccion_inmueble
                FROM contrato c
                JOIN inquilino i ON c.id_inquilino = i.id_inquilino
                JOIN inmueble im ON c.id_inmueble = im.id_inmueble
                WHERE c.id_contrato=@id;";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            return new Contrato
            {
                IdContrato   = rd.GetInt32("id_contrato"),
                Monto        = rd.GetDecimal("monto"),
                FechaDesde   = rd.GetDateTime("fecha_desde"),
                FechaHasta   = rd.GetDateTime("fecha_hasta"),
                IdInquilino  = rd.GetInt32("id_inquilino"),
                IdInmueble   = rd.GetInt32("id_inmueble"),
                Inquilino    = new Inquilino
                {
                    IdInquilino = rd.GetInt32("id_inquilino"),
                    Nombre      = rd.GetString("nombre"),
                    Apellido    = rd.GetString("apellido")
                },
                Inmueble = new Inmueble
                {
                    IdInmueble = rd.GetInt32("id_inmueble"),
                    Direccion  = rd.GetString("direccion_inmueble")
                }
            };
        }

        public IList<Contrato> Listar()
        {
            var lista = new List<Contrato>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            const string sql = @"
                SELECT c.id_contrato, c.monto, c.fecha_desde, c.fecha_hasta,
                       c.id_inquilino, i.nombre, i.apellido,
                       c.id_inmueble, im.direccion AS direccion_inmueble
                FROM contrato c
                JOIN inquilino i ON c.id_inquilino = i.id_inquilino
                JOIN inmueble im ON c.id_inmueble = im.id_inmueble
                ORDER BY c.fecha_desde DESC;";

            using var cmd = new MySqlCommand(sql, connection);
            using var rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                lista.Add(new Contrato
                {
                    IdContrato   = rd.GetInt32("id_contrato"),
                    Monto        = rd.GetDecimal("monto"),
                    FechaDesde   = rd.GetDateTime("fecha_desde"),
                    FechaHasta   = rd.GetDateTime("fecha_hasta"),
                    IdInquilino  = rd.GetInt32("id_inquilino"),
                    IdInmueble   = rd.GetInt32("id_inmueble"),
                    Inquilino    = new Inquilino
                    {
                        IdInquilino = rd.GetInt32("id_inquilino"),
                        Nombre      = rd.GetString("nombre"),
                        Apellido    = rd.GetString("apellido")
                    },
                    Inmueble = new Inmueble
                    {
                        IdInmueble = rd.GetInt32("id_inmueble"),
                        Direccion  = rd.GetString("direccion_inmueble")
                    }
                });
            }
            return lista;
        }
    }
}