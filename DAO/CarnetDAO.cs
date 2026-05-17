using MySqlConnector;
using System.Data;
using TP_ClubDeportivo.Data;
using TP_ClubDeportivo.Models;

namespace TP_ClubDeportivo.DAO
{
    internal class CarnetDAO
    {
        private readonly IConexionFactory _conexionFactory;

        public CarnetDAO()
            : this(new Conexion())
        {
        }

        public CarnetDAO(IConexionFactory conexionFactory)
        {
            _conexionFactory = conexionFactory;
        }

        public bool Crear(Carnet carnet, out int carnetId)
        {
            carnetId = 0;
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_emitir_carnet", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_socio_id", carnet.SocioId);
            command.Parameters.AddWithValue("@p_numero", carnet.Numero);
            command.Parameters.AddWithValue("@p_fecha_emision", carnet.FechaEmision);
            command.Parameters.AddWithValue("@p_fecha_vencimiento", carnet.FechaVencimiento);

            var outputParam = new MySqlParameter("@p_carnet_id", MySqlDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputParam);

            try
            {
                command.ExecuteNonQuery();
                if (int.TryParse(outputParam.Value?.ToString(), out var id))
                {
                    carnetId = id;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public Carnet? ObtenerPorSocio(int socioId)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_obtener_carnet_por_socio", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_socio_id", socioId);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapearCarnet(reader) : null;
        }

        private static Carnet MapearCarnet(MySqlDataReader reader)
        {
            return new Carnet
            {
                IdCarnet = reader.GetInt32("id_carnet"),
                SocioId = reader.GetInt32("socio_id"),
                Numero = reader.GetString("numero"),
                FechaEmision = reader.GetDateTime("fecha_emision"),
                FechaVencimiento = reader.GetDateTime("fecha_vencimiento"),
                Foto = reader.IsDBNull(reader.GetOrdinal("foto")) ? string.Empty : reader.GetString("foto")
            };
        }
    }
}
