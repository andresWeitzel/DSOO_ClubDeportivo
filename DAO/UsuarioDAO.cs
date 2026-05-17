using System.Data;
using MySqlConnector;
using TP_ClubDeportivo.Data;
using TP_ClubDeportivo.Models;

namespace TP_ClubDeportivo.DAO
{
    internal class UsuarioDAO
    {
        private readonly IConexionFactory _conexionFactory;

        public UsuarioDAO()
            : this(new Conexion())
        {
        }

        public UsuarioDAO(IConexionFactory conexionFactory)
        {
            _conexionFactory = conexionFactory;
        }

        public bool TestConexion()
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();
            return connection.State == ConnectionState.Open;
        }

        public Usuario? ObtenerPorUsername(string username)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_obtener_usuario_por_username", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_username", username);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapearUsuario(reader) : null;
        }

        public Usuario? Login(string username, string password)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("IngresoLogin", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Usu", username);
            command.Parameters.AddWithValue("@Pass", password);

            using var reader = command.ExecuteReader();
            return reader.Read() ? MapearUsuario(reader) : null;
        }

        private static Usuario MapearUsuario(MySqlDataReader reader)
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32("id_usuario"),
                Username = reader.GetString("username"),
                Password = reader.GetString("password"),
                Rol = reader.GetString("rol"),
                FechaRegistro = reader.GetDateTime("fecha_registro")
            };
        }
    }
}
