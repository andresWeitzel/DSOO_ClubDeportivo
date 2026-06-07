using MySqlConnector;

namespace TP_ClubDeportivo.Data
{
    public sealed class ResultadoInicializacionBaseDatos
    {
        public bool Exito { get; init; }
        public bool SeEjecutaronScripts { get; init; }
        public string Mensaje { get; init; } = string.Empty;
    }

    public static class InicializadorBaseDatos
    {
        public static bool BaseDeDatosLista()
        {
            if (!Conexion.EstaConfigurada)
            {
                return false;
            }

            try
            {
                using (var servidor = new MySqlConnection(Conexion.ObtenerCadenaServidor()))
                {
                    servidor.Open();

                    using var existeDb = new MySqlCommand(
                        """
                        SELECT COUNT(*)
                        FROM information_schema.SCHEMATA
                        WHERE SCHEMA_NAME = @nombre
                        """,
                        servidor);
                    existeDb.Parameters.AddWithValue("@nombre", Conexion.NombreBaseDatos);
                    if (Convert.ToInt64(existeDb.ExecuteScalar()) == 0)
                    {
                        return false;
                    }
                }

                using var connection = new MySqlConnection(Conexion.ObtenerCadenaConexion());
                connection.Open();

                using var existeLogin = new MySqlCommand(
                    """
                    SELECT COUNT(*)
                    FROM information_schema.ROUTINES
                    WHERE ROUTINE_SCHEMA = @nombre
                      AND ROUTINE_NAME = 'IngresoLogin'
                      AND ROUTINE_TYPE = 'PROCEDURE'
                    """,
                    connection);
                existeLogin.Parameters.AddWithValue("@nombre", Conexion.NombreBaseDatos);
                if (Convert.ToInt64(existeLogin.ExecuteScalar()) == 0)
                {
                    return false;
                }

                using var usuarios = new MySqlCommand("SELECT COUNT(*) FROM usuario", connection);
                return Convert.ToInt64(usuarios.ExecuteScalar()) > 0;
            }
            catch
            {
                return false;
            }
        }

        public static ResultadoInicializacionBaseDatos EjecutarSiEsNecesario(Action<ProgresoInicializacionBaseDatos>? progreso = null)
        {
            progreso?.Invoke(new ProgresoInicializacionBaseDatos
            {
                MensajeAmigable = "Verificando la base de datos del club deportivo…",
                EsMensajeSistema = true
            });

            if (BaseDeDatosLista())
            {
                return new ResultadoInicializacionBaseDatos
                {
                    Exito = true,
                    SeEjecutaronScripts = false,
                    Mensaje = "La base de datos ya está configurada."
                };
            }

            return EjecutarScripts(progreso);
        }

        public static ResultadoInicializacionBaseDatos EjecutarScripts(Action<ProgresoInicializacionBaseDatos>? progreso = null)
        {
            try
            {
                var carpetaScripts = ResolverCarpetaScripts();
                var archivos = Directory.GetFiles(carpetaScripts, "*.sql")
                    .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (archivos.Count == 0)
                {
                    return new ResultadoInicializacionBaseDatos
                    {
                        Exito = false,
                        SeEjecutaronScripts = false,
                        Mensaje = $"No se encontraron scripts SQL en {carpetaScripts}."
                    };
                }

                if (EjecutorScriptsMySql.IntentarEjecutarConCliente(archivos, progreso, out var errorCliente))
                {
                    return FinalizarResultado(true, "Base de datos inicializada con el cliente MySQL.");
                }

                if (!string.IsNullOrWhiteSpace(errorCliente))
                {
                    progreso?.Invoke(new ProgresoInicializacionBaseDatos
                    {
                        MensajeAmigable = "Reintentando la carga desde la aplicación…",
                        EsMensajeSistema = true
                    });
                    try
                    {
                        EjecutorScriptsMySql.EjecutarConParser(archivos, progreso);
                        return FinalizarResultado(true, "Base de datos inicializada desde la aplicación.");
                    }
                    catch
                    {
                        return new ResultadoInicializacionBaseDatos
                        {
                            Exito = false,
                            SeEjecutaronScripts = true,
                            Mensaje = "Falló el cliente MySQL al ejecutar los scripts.\n" + errorCliente.Trim()
                        };
                    }
                }

                progreso?.Invoke(new ProgresoInicializacionBaseDatos
                {
                    MensajeAmigable = "Preparando la carga de datos en el sistema…",
                    EsMensajeSistema = true
                });
                EjecutorScriptsMySql.EjecutarConParser(archivos, progreso);
                return FinalizarResultado(true, "Base de datos inicializada desde la aplicación.");
            }
            catch (Exception ex)
            {
                return new ResultadoInicializacionBaseDatos
                {
                    Exito = false,
                    SeEjecutaronScripts = true,
                    Mensaje = ex.Message
                };
            }
        }

        private static ResultadoInicializacionBaseDatos FinalizarResultado(bool scriptsEjecutados, string mensajeOk)
        {
            if (!BaseDeDatosLista())
            {
                return new ResultadoInicializacionBaseDatos
                {
                    Exito = false,
                    SeEjecutaronScripts = scriptsEjecutados,
                    Mensaje = "Los scripts se ejecutaron, pero la base no quedó lista para el login."
                };
            }

            return new ResultadoInicializacionBaseDatos
            {
                Exito = true,
                SeEjecutaronScripts = scriptsEjecutados,
                Mensaje = mensajeOk
            };
        }

        public static string ResolverCarpetaScripts()
        {
            var candidatos = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "Database", "Scripts"),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Database", "Scripts")),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Database", "Scripts"))
            };

            foreach (var candidato in candidatos)
            {
                if (Directory.Exists(candidato))
                {
                    return candidato;
                }
            }

            throw new DirectoryNotFoundException(
                "No se encontró la carpeta Database/Scripts junto al ejecutable ni en el proyecto.");
        }
    }
}
