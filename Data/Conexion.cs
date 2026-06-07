using MySqlConnector;

namespace TP_ClubDeportivo.Data
{
    public class Conexion : IConexionFactory
    {
        public const string NombreBaseDatos = "db_club_deportivo";

        private static string? _cadenaConexion;
        private static DatosInstalacionMySql? _datosActuales;

        public static bool EstaConfigurada => _datosActuales is not null;

        public static DatosInstalacionMySql? ObtenerDatosInstalacion() => _datosActuales;

        public static void EstablecerConfiguracion(DatosInstalacionMySql datos)
        {
            _datosActuales = datos with { BaseDatos = NombreBaseDatos };
            _cadenaConexion = ArmarCadenaConexion(_datosActuales);
        }

        public static bool ProbarConexionActual(out string mensaje)
        {
            if (!EstaConfigurada)
            {
                mensaje = "Aún no se configuraron los datos de conexión.";
                return false;
            }

            if (InicializadorBaseDatos.BaseDeDatosLista())
            {
                return new ConnectionTester(new Conexion()).Test(out mensaje);
            }

            try
            {
                using var connection = new MySqlConnection(ObtenerCadenaServidor());
                connection.Open();
                mensaje = "Conexión al servidor MySQL establecida. La base de datos aún no está inicializada.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        public static string ObtenerCadenaConexion()
        {
            if (!EstaConfigurada || string.IsNullOrEmpty(_cadenaConexion))
            {
                throw new InvalidOperationException(
                    "Debe configurar los datos de instalación MySQL antes de usar el sistema.");
            }

            return _cadenaConexion;
        }

        /// <summary>
        /// Conexión al servidor sin base seleccionada (útil para CREATE/DROP DATABASE).
        /// </summary>
        public static string ObtenerCadenaServidor()
        {
            if (_datosActuales is null)
            {
                throw new InvalidOperationException(
                    "Debe configurar los datos de instalación MySQL antes de usar el sistema.");
            }

            var builder = new MySqlConnectionStringBuilder
            {
                Server = _datosActuales.Servidor.Trim(),
                Port = uint.TryParse(_datosActuales.Puerto.Trim(), out var puertoNumerico) ? puertoNumerico : 3306,
                UserID = _datosActuales.Usuario.Trim(),
                Password = _datosActuales.Clave
            };

            return builder.ConnectionString;
        }

        public MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(ObtenerCadenaConexion());
        }

        private static string ArmarCadenaConexion(DatosInstalacionMySql datos)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = datos.Servidor.Trim(),
                Port = uint.TryParse(datos.Puerto.Trim(), out var puertoNumerico) ? puertoNumerico : 3306,
                UserID = datos.Usuario.Trim(),
                Password = datos.Clave,
                Database = NombreBaseDatos
            };

            return builder.ConnectionString;
        }
    }
}
