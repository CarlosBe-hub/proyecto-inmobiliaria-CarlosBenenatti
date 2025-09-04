using MySql.Data.MySqlClient;
using ProyectoInmobiliaria.Models;

namespace ProyectoInmobiliaria.Repository
{
    public class ContratoRepository : RepositorioBase, IContratoRepository
    {
        public ContratoRepository(IConfiguration configuration) : base(configuration) { }

        // Alta
        public void Alta(Contrato contrato)
        {
            using (var conn = GetConnection())
            {
                var sql = @"INSERT INTO contratos 
                            (FechaInicio, FechaFin, Monto, InmuebleId, InquilinoId) 
                            VALUES (@inicio, @fin, @monto, @inmuebleId, @inquilinoId)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@inicio", contrato.FechaInicio);
                    cmd.Parameters.AddWithValue("@fin", contrato.FechaFin);
                    cmd.Parameters.AddWithValue("@monto", contrato.Monto);
                    cmd.Parameters.AddWithValue("@inmuebleId", contrato.InmuebleId);
                    cmd.Parameters.AddWithValue("@inquilinoId", contrato.InquilinoId);
                    cmd.ExecuteNonQuery();
                    contrato.IdContrato = (int)cmd.LastInsertedId;
                }
            }
        }

        // Baja
        public void Baja(int id)
        {
            using (var conn = GetConnection())
            {
                var sql = "DELETE FROM contratos WHERE IdContrato = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Modificar
        public void Modificar(Contrato contrato)
        {
            using (var conn = GetConnection())
            {
                var sql = @"UPDATE contratos 
                            SET FechaInicio = @inicio, FechaFin = @fin, 
                                Monto = @monto, InmuebleId = @inmuebleId, InquilinoId = @inquilinoId 
                            WHERE IdContrato = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@inicio", contrato.FechaInicio);
                    cmd.Parameters.AddWithValue("@fin", contrato.FechaFin);
                    cmd.Parameters.AddWithValue("@monto", contrato.Monto);
                    cmd.Parameters.AddWithValue("@inmuebleId", contrato.InmuebleId);
                    cmd.Parameters.AddWithValue("@inquilinoId", contrato.InquilinoId);
                    cmd.Parameters.AddWithValue("@id", contrato.IdContrato);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Obtener por Id
        public Contrato? ObtenerPorId(int id)
        {
            Contrato? contrato = null;
            using (var conn = GetConnection())
            {
                var sql = @"SELECT c.IdContrato, c.FechaInicio, c.FechaFin, c.Monto, 
                                   c.InmuebleId, c.InquilinoId,
                                   i.Direccion, q.Nombre, q.Apellido
                            FROM contratos c
                            INNER JOIN inmueble i ON c.InmuebleId = i.id_inmueble
                            INNER JOIN inquilino q ON c.InquilinoId = q.IdInquilino
                            WHERE c.IdContrato = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            contrato = new Contrato
                            {
                                IdContrato = reader.GetInt32("IdContrato"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                Monto = reader.GetDecimal("Monto"),
                                InmuebleId = reader.GetInt32("InmuebleId"),
                                InquilinoId = reader.GetInt32("InquilinoId"),
                                Inmueble = new Inmueble
                                {
                                    IdInmueble = reader.GetInt32("InmuebleId"),
                                    Direccion = reader.GetString("Direccion")
                                },
                                Inquilino = new Inquilino
                                {
                                    IdInquilino = reader.GetInt32("InquilinoId"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido")
                                }
                            };
                        }
                    }
                }
            }
            return contrato;
        }

        // Listar todos
        public IList<Contrato> Listar()
        {
            var lista = new List<Contrato>();
            using (var conn = GetConnection())
            {
                var sql = @"SELECT c.IdContrato, c.FechaInicio, c.FechaFin, c.Monto, 
                                   c.InmuebleId, c.InquilinoId,
                                   i.Direccion, q.Nombre, q.Apellido
                            FROM contratos c
                            INNER JOIN inmueble i ON c.InmuebleId = i.id_inmueble
                            INNER JOIN inquilino q ON c.InquilinoId = q.IdInquilino";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var contrato = new Contrato
                            {
                                IdContrato = reader.GetInt32("IdContrato"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                Monto = reader.GetDecimal("Monto"),
                                InmuebleId = reader.GetInt32("InmuebleId"),
                                InquilinoId = reader.GetInt32("InquilinoId"),
                                Inmueble = new Inmueble
                                {
                                    IdInmueble = reader.GetInt32("InmuebleId"),
                                    Direccion = reader.GetString("Direccion")
                                },
                                Inquilino = new Inquilino
                                {
                                    IdInquilino = reader.GetInt32("InquilinoId"),
                                    Nombre = reader.GetString("Nombre"),
                                    Apellido = reader.GetString("Apellido")
                                }
                            };
                            lista.Add(contrato);
                        }
                    }
                }
            }
            return lista;
        }

        // Validar ocupación de inmueble en rango de fechas
        public bool ExisteOcupacion(int inmuebleId, DateTime fechaInicio, DateTime fechaFin, int? idContratoExcluir = null)
        {
            using (var conn = GetConnection())
            {
                var sql = @"SELECT COUNT(*) 
                            FROM contratos 
                            WHERE InmuebleId = @inmuebleId
                              AND (@idContrato IS NULL OR IdContrato <> @idContrato)
                              AND (
                                    (@fechaInicio BETWEEN FechaInicio AND FechaFin)
                                 OR (@fechaFin BETWEEN FechaInicio AND FechaFin)
                                 OR (FechaInicio BETWEEN @fechaInicio AND @fechaFin)
                                 OR (FechaFin BETWEEN @fechaInicio AND @fechaFin)
                                  )";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                    cmd.Parameters.AddWithValue("@idContrato", (object?)idContratoExcluir ?? DBNull.Value);

                    var result = Convert.ToInt32(cmd.ExecuteScalar());
                    return result > 0;
                }
            }
        }
    }
}
