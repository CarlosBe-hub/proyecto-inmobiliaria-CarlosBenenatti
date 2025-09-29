using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ProyectoInmobiliaria.Models;
using ProyectoInmobiliaria.Repository;
using Microsoft.Extensions.Configuration;

namespace ProyectoInmobiliaria.Repository
{
    public class RepositorioInmueble : RepositorioBase, IInmuebleRepository
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Inmueble i)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                var sql = @"INSERT INTO Inmueble (direccion, tipo_inmueble, estado, ambientes, superficie, precio, propietario_id)
                            VALUES (@Direccion, @TipoInmueble, @Estado, @Ambientes, @Superficie, @Precio, @PropietarioId);
                            SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Direccion", i.Direccion);
                    command.Parameters.AddWithValue("@TipoInmueble", i.TipoInmueble);
                    command.Parameters.AddWithValue("@Estado", i.Estado);
                    command.Parameters.AddWithValue("@Ambientes", i.Ambientes);
                    command.Parameters.AddWithValue("@Superficie", i.Superficie);
                    command.Parameters.AddWithValue("@Precio", i.Precio);
                    command.Parameters.AddWithValue("@PropietarioId", i.PropietarioId);
                    res = Convert.ToInt32(command.ExecuteScalar());
                    i.IdInmueble = res;
                }
            }
            return res;
        }

        public int Baja(int idInmueble)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                var sql = "DELETE FROM Inmueble WHERE id_inmueble = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idInmueble);
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificar(Inmueble i)
        {
            int res = -1;
            using (var connection = GetConnection())
            {
                var sql = @"UPDATE Inmueble SET direccion=@Direccion, tipo_inmueble=@TipoInmueble,
                            estado=@Estado, ambientes=@Ambientes, superficie=@Superficie, 
                            precio=@Precio, propietario_id=@PropietarioId 
                            WHERE id_inmueble=@IdInmueble";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Direccion", i.Direccion);
                    command.Parameters.AddWithValue("@TipoInmueble", i.TipoInmueble);
                    command.Parameters.AddWithValue("@Estado", i.Estado);
                    command.Parameters.AddWithValue("@Ambientes", i.Ambientes);
                    command.Parameters.AddWithValue("@Superficie", i.Superficie);
                    command.Parameters.AddWithValue("@Precio", i.Precio);
                    command.Parameters.AddWithValue("@PropietarioId", i.PropietarioId);
                    command.Parameters.AddWithValue("@IdInmueble", i.IdInmueble);
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public Inmueble? ObtenerPorId(int idInmueble)
        {
            Inmueble? inmueble = null;
            using (var connection = GetConnection())
            {
                var sql = @"SELECT i.id_inmueble, i.direccion, i.tipo_inmueble, i.estado, i.ambientes, 
                                   i.superficie, i.precio, i.propietario_id,
                                   p.id_propietario, p.nombre, p.apellido
                            FROM Inmueble i 
                            INNER JOIN Propietario p ON i.propietario_id = p.id_propietario
                            WHERE i.id_inmueble=@id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idInmueble);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                TipoInmueble = reader.GetString("tipo_inmueble"),
                                Estado = reader.GetString("estado"),
                                Ambientes = reader.GetInt32("ambientes"),
                                Superficie = reader.GetInt32("superficie"),
                                Precio = reader.GetDecimal("precio"),
                                PropietarioId = reader.GetInt32("propietario_id"),
                                Propietario = new Propietario
                                {
                                    IdPropietario = reader.GetInt32("id_propietario"),
                                    Nombre = reader.GetString("nombre"),
                                    Apellido = reader.GetString("apellido")
                                }
                            };
                        }
                    }
                }
            }
            return inmueble;
        }

        // Listar sin paginación
        public IList<Inmueble> Listar()    
        {
            var res = new List<Inmueble>();
            using (var connection = GetConnection())
            {
                var sql = @"SELECT i.id_inmueble, i.direccion, i.tipo_inmueble, i.estado, i.ambientes, 
                                   i.superficie, i.precio, i.propietario_id,
                                   p.id_propietario, p.nombre, p.apellido
                            FROM Inmueble i 
                            INNER JOIN Propietario p ON i.propietario_id = p.id_propietario";
                using (var command = new MySqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                TipoInmueble = reader.GetString("tipo_inmueble"),
                                Estado = reader.GetString("estado"),
                                Ambientes = reader.GetInt32("ambientes"),
                                Superficie = reader.GetInt32("superficie"),
                                Precio = reader.GetDecimal("precio"),
                                PropietarioId = reader.GetInt32("propietario_id"),
                                Propietario = new Propietario
                                {
                                    IdPropietario = reader.GetInt32("id_propietario"),
                                    Nombre = reader.GetString("nombre"),
                                    Apellido = reader.GetString("apellido")
                                }
                            };
                            res.Add(inmueble);
                        }
                    }
                }
            }
            return res;
        }

        // Listar con paginación
        public IList<Inmueble> Listar(int pagina, int tamPagina)
        {
            var res = new List<Inmueble>();
            using (var connection = GetConnection())
            {
                var sql = @"SELECT i.id_inmueble, i.direccion, i.tipo_inmueble, i.estado, i.ambientes, 
                                   i.superficie, i.precio, i.propietario_id,
                                   p.id_propietario, p.nombre, p.apellido
                            FROM Inmueble i 
                            INNER JOIN Propietario p ON i.propietario_id = p.id_propietario
                            LIMIT @limit OFFSET @offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@limit", tamPagina);
                    command.Parameters.AddWithValue("@offset", (pagina - 1) * tamPagina);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                TipoInmueble = reader.GetString("tipo_inmueble"),
                                Estado = reader.GetString("estado"),
                                Ambientes = reader.GetInt32("ambientes"),
                                Superficie = reader.GetInt32("superficie"),
                                Precio = reader.GetDecimal("precio"),
                                PropietarioId = reader.GetInt32("propietario_id"),
                                Propietario = new Propietario
                                {
                                    IdPropietario = reader.GetInt32("id_propietario"),
                                    Nombre = reader.GetString("nombre"),
                                    Apellido = reader.GetString("apellido")
                                }
                            };
                            res.Add(inmueble);
                        }
                    }
                }
            }
            return res;
        }

        public IList<Inmueble> BuscarPorPropietario(int propietarioId)
        {
            var res = new List<Inmueble>();
            using (var connection = GetConnection())
            {
                var sql = @"SELECT i.id_inmueble, i.direccion, i.tipo_inmueble, i.estado, i.ambientes, 
                                   i.superficie, i.precio, i.propietario_id,
                                   p.id_propietario, p.nombre, p.apellido
                            FROM Inmueble i
                            INNER JOIN Propietario p ON i.propietario_id = p.id_propietario
                            WHERE i.propietario_id=@propietarioId";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@propietarioId", propietarioId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32("id_inmueble"),
                                Direccion = reader.GetString("direccion"),
                                TipoInmueble = reader.GetString("tipo_inmueble"),
                                Estado = reader.GetString("estado"),
                                Ambientes = reader.GetInt32("ambientes"),
                                Superficie = reader.GetInt32("superficie"),
                                Precio = reader.GetDecimal("precio"),
                                PropietarioId = reader.GetInt32("propietario_id"),
                                Propietario = new Propietario
                                {
                                    IdPropietario = reader.GetInt32("id_propietario"),
                                    Nombre = reader.GetString("nombre"),
                                    Apellido = reader.GetString("apellido")
                                }
                            };
                            res.Add(inmueble);
                        }
                    }
                }
            }
            return res;
        }
    }
}
