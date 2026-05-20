using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using TP_ClubDeportivo.Data;
using TP_ClubDeportivo.Models;

namespace TP_ClubDeportivo.DAO
{
    internal class ReporteDAO
    {
        private readonly IConexionFactory _conexionFactory;

        public ReporteDAO()
            : this(new Conexion())
        {
        }

        public ReporteDAO(IConexionFactory conexionFactory)
        {
            _conexionFactory = conexionFactory;
        }

        public IEnumerable<CuotaReporte> ObtenerCuotasPorVencer(int dias)
        {
            if (dias < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(dias), "Los días deben ser al menos 1.");
            }

            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_cuotas_por_vencer", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_dias", dias);

            using var reader = command.ExecuteReader();
            var lista = new List<CuotaReporte>();
            while (reader.Read())
            {
                lista.Add(MapearCuotaReporte(reader, "dias_para_vencer"));
            }

            return lista;
        }

        public IEnumerable<CuotaReporte> ObtenerCuotasVencidas()
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_cuotas_vencidas", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            using var reader = command.ExecuteReader();
            var lista = new List<CuotaReporte>();
            while (reader.Read())
            {
                lista.Add(MapearCuotaReporte(reader, "dias_vencidos"));
            }

            return lista;
        }

        private static CuotaReporte MapearCuotaReporte(MySqlDataReader reader, string columnaDias)
        {
            return new CuotaReporte
            {
                IdSocio = reader.GetInt32("id_socio"),
                Dni = reader.GetString("dni"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                IdCuota = reader.GetInt32("id_cuota"),
                Monto = reader.GetDecimal("monto"),
                FechaVencimiento = reader.GetDateTime("fecha_vencimiento"),
                Dias = reader.GetInt32(columnaDias)
            };
        }
    }
}
