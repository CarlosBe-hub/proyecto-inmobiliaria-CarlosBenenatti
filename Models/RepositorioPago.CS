using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ProyectoInmobiliaria.Models;
using Microsoft.Extensions.Configuration;

namespace ProyectoInmobiliaria.Repository
{
public class RepositorioPago : RepositorioBase, IPagoRepository
{
public RepositorioPago(IConfiguration configuration) : base(configuration)
{
}

    public int Alta(Pago pago)  
    {  
        int res = -1;  
        using (var connection = GetConnection())  
        {  
            var sql = @"INSERT INTO Pago (id_contrato, metodo_pago, fecha_pago, monto, detalle, anulado, numero_pago, pagado)   
                        VALUES (@idContrato, @metodo, @fecha, @monto, @detalle, 0, @numero, @pagado);  
                        SELECT LAST_INSERT_ID();";  

            using (var command = new MySqlCommand(sql, connection))  
            {  
                command.Parameters.AddWithValue("@idContrato", pago.IdContrato);  
                command.Parameters.AddWithValue("@metodo", pago.MetodoPago ?? (object)DBNull.Value);  
                command.Parameters.AddWithValue("@fecha", pago.FechaPago);  
                command.Parameters.AddWithValue("@monto", pago.Monto);  
                command.Parameters.AddWithValue("@detalle", pago.Detalle ?? (object)DBNull.Value);  
                command.Parameters.AddWithValue("@numero", pago.NumeroPago);  
                command.Parameters.AddWithValue("@pagado", pago.Pagado);  

                res = Convert.ToInt32(command.ExecuteScalar());  
                pago.IdPago = res;  
            }  
        }  
        return res;  
    }  

    public int Baja(int idPago)  
    {  
        int res = -1;  
        using (var connection = GetConnection())  
        {  
            var sql = "UPDATE Pago SET anulado = 1 WHERE id_pago = @id";  
            using (var command = new MySqlCommand(sql, connection))  
            {  
                command.Parameters.AddWithValue("@id", idPago);  
                res = command.ExecuteNonQuery();  
            }  
        }  
        return res;  
    }  

    public int Modificar(Pago pago)  
    {  
        int res = -1;  
        using (var connection = GetConnection())  
        {  
            var sql = @"UPDATE Pago   
                        SET metodo_pago=@metodo, fecha_pago=@fecha, monto=@monto, detalle=@detalle, pagado=@pagado  
                        WHERE id_pago=@idPago";  

            using (var command = new MySqlCommand(sql, connection))  
            {  
                command.Parameters.AddWithValue("@metodo", pago.MetodoPago ?? (object)DBNull.Value);  
                command.Parameters.AddWithValue("@fecha", pago.FechaPago);  
                command.Parameters.AddWithValue("@monto", pago.Monto);  
                command.Parameters.AddWithValue("@detalle", pago.Detalle ?? (object)DBNull.Value);  
                command.Parameters.AddWithValue("@pagado", pago.Pagado);  
                command.Parameters.AddWithValue("@idPago", pago.IdPago);  
                res = command.ExecuteNonQuery();  
            }  
        }  
        return res;  
    }  

    public Pago? ObtenerPorId(int idPago)  
    {  
        Pago? pago = null;  
        using (var connection = GetConnection())  
        {  
            var sql = @"SELECT id_pago, id_contrato, metodo_pago, fecha_pago, monto, detalle, anulado, numero_pago, pagado  
                        FROM Pago WHERE id_pago = @id";  

            using (var command = new MySqlCommand(sql, connection))  
            {  
                command.Parameters.AddWithValue("@id", idPago);  
                using (var reader = command.ExecuteReader())  
                {  
                    if (reader.Read())  
                    {  
                        pago = new Pago  
                        {  
                            IdPago = reader.GetInt32("id_pago"),  
                            IdContrato = reader.GetInt32("id_contrato"),  
                            MetodoPago = reader.IsDBNull(reader.GetOrdinal("metodo_pago")) ? null : reader.GetString("metodo_pago"),  
                            FechaPago = reader.GetDateTime("fecha_pago"),  
                            Monto = reader.GetDecimal("monto"),  
                            Detalle = reader.IsDBNull(reader.GetOrdinal("detalle")) ? null : reader.GetString("detalle"),  
                            Anulado = reader.GetBoolean("anulado"),  
                            NumeroPago = reader.GetInt32("numero_pago"),  
                            Pagado = reader.GetBoolean("pagado")  
                        };  
                    }  
                }  
            }  
        }  
        return pago;  
    }  

    public IList<Pago> ObtenerPorContrato(int idContrato)  
    {  
        var res = new List<Pago>();  
        using (var connection = GetConnection())  
        {  
            var sql = @"SELECT id_pago, id_contrato, metodo_pago, fecha_pago, monto, detalle, anulado, numero_pago, pagado  
                        FROM Pago   
                        WHERE id_contrato=@idContrato AND anulado=0  
                        ORDER BY numero_pago";  

            using (var command = new MySqlCommand(sql, connection))  
            {  
                command.Parameters.AddWithValue("@idContrato", idContrato);  
                using (var reader = command.ExecuteReader())  
                {  
                    while (reader.Read())  
                    {  
                        var pago = new Pago  
                        {  
                            IdPago = reader.GetInt32("id_pago"),  
                            IdContrato = reader.GetInt32("id_contrato"),  
                            MetodoPago = reader.IsDBNull(reader.GetOrdinal("metodo_pago")) ? null : reader.GetString("metodo_pago"),  
                            FechaPago = reader.GetDateTime("fecha_pago"),  
                            Monto = reader.GetDecimal("monto"),  
                            Detalle = reader.IsDBNull(reader.GetOrdinal("detalle")) ? null : reader.GetString("detalle"),  
                            Anulado = reader.GetBoolean("anulado"),  
                            NumeroPago = reader.GetInt32("numero_pago"),  
                            Pagado = reader.GetBoolean("pagado")  
                        };  
                        res.Add(pago);  
                    }  
                }  
            }  
        }  
        return res;  
    }  

    public int ObtenerUltimoNumeroPago(int idContrato)  
    {  
        int numero = 0;  
        using (var connection = GetConnection())  
        {  
            var sql = "SELECT IFNULL(MAX(numero_pago), 0) FROM Pago WHERE id_contrato = @idContrato";  
            using (var command = new MySqlCommand(sql, connection))  
            {  
                command.Parameters.AddWithValue("@idContrato", idContrato);  
                numero = Convert.ToInt32(command.ExecuteScalar());  
            }  
        }  
        return numero;  
    }  
}  
}
