using System.Diagnostics;
using System.Text;
using MySqlConnector;

namespace TP_ClubDeportivo.Data
{
    internal static class EjecutorScriptsMySql
    {
        public static bool IntentarEjecutarConCliente(
            IReadOnlyList<string> archivos,
            Action<ProgresoInicializacionBaseDatos>? progreso,
            out string? error)
        {
            error = null;
            var mysqlExe = BuscarEjecutableMysql();
            if (mysqlExe is null)
            {
                return false;
            }

            var datos = Conexion.ObtenerDatosInstalacion()
                ?? throw new InvalidOperationException("No hay datos de conexión configurados.");

            var argumentos = ArmarArgumentosCliente(datos);

            for (var i = 0; i < archivos.Count; i++)
            {
                var archivo = archivos[i];
                var nombreArchivo = Path.GetFileName(archivo);
                NotificarProgreso(progreso, i + 1, archivos.Count, nombreArchivo);

                if (!EjecutarArchivoConCmd(mysqlExe, argumentos, archivo, out var errorArchivo))
                {
                    error = $"Error en {nombreArchivo}:\n{errorArchivo}";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Ejecuta mysql archivo.sql vía cmd, igual que init_db.bat (evita deadlock de stdin).
        /// </summary>
        private static bool EjecutarArchivoConCmd(
            string mysqlExe,
            string argumentosMysql,
            string archivoSql,
            out string error)
        {
            error = string.Empty;

            var comando = $"\"\"{mysqlExe}\" {argumentosMysql} < \"{archivoSql}\"\"";

            using var proceso = new Process();
            proceso.StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {comando}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            proceso.Start();

            var leerSalida = Task.Run(() => proceso.StandardOutput.ReadToEnd());
            var leerError = Task.Run(() => proceso.StandardError.ReadToEnd());

            proceso.WaitForExit();

            var salida = leerSalida.GetAwaiter().GetResult();
            var salidaError = leerError.GetAwaiter().GetResult();

            if (proceso.ExitCode != 0)
            {
                error = FiltrarMensajeError(salidaError, salida);
                if (string.IsNullOrWhiteSpace(error))
                {
                    error = $"El cliente MySQL finalizó con código {proceso.ExitCode}.";
                }

                return false;
            }

            return true;
        }

        private static string FiltrarMensajeError(string stderr, string stdout)
        {
            var lineas = (stderr + Environment.NewLine + stdout)
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Where(l => !l.Contains("ssl-verify-server-cert", StringComparison.OrdinalIgnoreCase))
                .Where(l => !l.StartsWith("WARNING:", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return lineas.Count == 0 ? string.Empty : string.Join(Environment.NewLine, lineas);
        }

        public static void EjecutarConParser(
            IReadOnlyList<string> archivos,
            Action<ProgresoInicializacionBaseDatos>? progreso)
        {
            using var connection = new MySqlConnection(Conexion.ObtenerCadenaServidor());
            connection.Open();

            for (var i = 0; i < archivos.Count; i++)
            {
                var archivo = archivos[i];
                var nombreArchivo = Path.GetFileName(archivo);
                NotificarProgreso(progreso, i + 1, archivos.Count, nombreArchivo);

                var contenido = File.ReadAllText(archivo);
                foreach (var sentencia in MySqlScriptParser.Dividir(contenido))
                {
                    using var command = new MySqlCommand(sentencia, connection)
                    {
                        CommandTimeout = 180
                    };
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void NotificarProgreso(
            Action<ProgresoInicializacionBaseDatos>? progreso,
            int paso,
            int total,
            string nombreArchivo)
        {
            progreso?.Invoke(new ProgresoInicializacionBaseDatos
            {
                Paso = paso,
                Total = total,
                NombreArchivo = nombreArchivo,
                MensajeAmigable = MensajesScriptsBaseDatos.ObtenerMensajeAmigable(nombreArchivo)
            });
        }

        private static string ArmarArgumentosCliente(DatosInstalacionMySql datos)
        {
            var args = new StringBuilder();
            args.Append("-h ").Append(Quote(datos.Servidor.Trim()));
            args.Append(" -P ").Append(datos.Puerto.Trim());
            args.Append(" -u ").Append(Quote(datos.Usuario.Trim()));
            args.Append(" --default-character-set=utf8mb4 --batch");

            if (!string.IsNullOrEmpty(datos.Clave))
            {
                args.Append(" -p").Append(Quote(datos.Clave));
            }

            return args.ToString();
        }

        private static string Quote(string valor) =>
            valor.Contains(' ') ? $"\"{valor}\"" : valor;

        private static string? BuscarEjecutableMysql()
        {
            var candidatos = new List<string>();

            var pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (!string.IsNullOrWhiteSpace(pathEnv))
            {
                foreach (var carpeta in pathEnv.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    candidatos.Add(Path.Combine(carpeta.Trim(), "mysql.exe"));
                }
            }

            candidatos.AddRange(
            [
                @"C:\Program Files\MariaDB 11.6\bin\mysql.exe",
                @"C:\Program Files\MariaDB 11.4\bin\mysql.exe",
                @"C:\Program Files\MariaDB 10.11\bin\mysql.exe",
                @"C:\Program Files\MySQL\MySQL Server 8.4\bin\mysql.exe",
                @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe"
            ]);

            foreach (var candidato in candidatos.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (File.Exists(candidato))
                {
                    return candidato;
                }
            }

            return null;
        }
    }
}
