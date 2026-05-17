using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using TP_ClubDeportivo.Data;
using TP_ClubDeportivo.Models;

namespace TP_ClubDeportivo.DAO
{
    internal class CuotaDAO
    {
        private readonly IConexionFactory _conexionFactory;

        public CuotaDAO()
            : this(new Conexion())
        {
        }

        public CuotaDAO(IConexionFactory conexionFactory)
        {
            _conexionFactory = conexionFactory;
        }

        public IEnumerable<Cuota> ObtenerPorSocio(int socioId)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_obtener_cuotas_por_socio", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_socio_id", socioId);

            using var reader = command.ExecuteReader();
            var lista = new List<Cuota>();
            while (reader.Read())
            {
                lista.Add(MapearCuota(reader));
            }

            return lista;
        }

        public Cuota? ObtenerUltimaPorSocio(int socioId)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_obtener_ultima_cuota_socio", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_socio_id", socioId);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapearCuota(reader) : null;
        }

        public bool Crear(Cuota cuota, out int cuotaId)
        {
            cuotaId = 0;
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_crear_cuota", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_socio_id", cuota.SocioId);
            command.Parameters.AddWithValue("@p_monto", cuota.Monto);
            command.Parameters.AddWithValue("@p_fecha_vencimiento", cuota.FechaVencimiento);

            var outputParam = new MySqlParameter("@p_cuota_id", MySqlDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputParam);

            try
            {
                command.ExecuteNonQuery();
                if (int.TryParse(outputParam.Value?.ToString(), out var id))
                {
                    cuotaId = id;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool ActualizarEstado(int cuotaId, string estado, bool enMora)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_actualizar_estado_cuota_mora", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_id_cuota", cuotaId);
            command.Parameters.AddWithValue("@p_estado", estado);
            command.Parameters.AddWithValue("@p_en_mora", enMora);

            try
            {
                return command.ExecuteNonQuery() >= 1;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Cuota> ObtenerPorVencerEnDias(int dias)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_cuotas_por_vencer", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_dias", dias);

            using var reader = command.ExecuteReader();
            var lista = new List<Cuota>();
            while (reader.Read())
            {
                lista.Add(MapearCuota(reader));
            }

            return lista;
        }

        public IEnumerable<Cuota> ObtenerVencidas()
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_cuotas_vencidas", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            using var reader = command.ExecuteReader();
            var lista = new List<Cuota>();
            while (reader.Read())
            {
                lista.Add(MapearCuota(reader));
            }

            return lista;
        }

        private static Cuota MapearCuota(MySqlDataReader reader)
        {
            return new Cuota
            {
                IdCuota = reader.GetInt32("id_cuota"),
                SocioId = reader.GetInt32("socio_id"),
                Monto = reader.GetDecimal("monto"),
                FechaEmision = reader.GetDateTime("fecha_emision"),
                FechaVencimiento = reader.GetDateTime("fecha_vencimiento"),
                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? string.Empty : reader.GetString("estado"),
                EnMora = reader.GetBoolean("en_mora")
            };
        }
    }
}
