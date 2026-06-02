using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using MySqlConnector;
using TP_ClubDeportivo.Data;

namespace TP_ClubDeportivo.DAO
{
    internal class TurnoNutricionDAO
    {
        private static readonly TimeSpan[] FranjasNutricion =
        [
            new(9, 0, 0), new(9, 30, 0), new(10, 0, 0), new(10, 30, 0),
            new(11, 0, 0), new(11, 30, 0),
            new(14, 0, 0), new(14, 30, 0), new(15, 0, 0), new(15, 30, 0),
            new(16, 0, 0), new(16, 30, 0), new(17, 0, 0)
        ];

        private static readonly string[] FormatosHora =
        [
            @"hh\:mm\:ss",
            @"h\:mm\:ss",
            @"hh\:mm",
            @"h\:mm"
        ];

        private readonly IConexionFactory _conexionFactory;

        public TurnoNutricionDAO()
            : this(new Conexion())
        {
        }

        public TurnoNutricionDAO(IConexionFactory conexionFactory)
        {
            _conexionFactory = conexionFactory;
        }

        public IEnumerable<(int Id, int SocioId, string SocioNombre, int NutricionistaId, string NutricionistaNombre, DateTime Fecha, TimeSpan Hora, string Estado)> ObtenerPorSocio(int socioId)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_obtener_turnos_socio", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_socio_id", socioId);

            using var reader = command.ExecuteReader();

            var lista = new List<(int, int, string, int, string, DateTime, TimeSpan, string)>();
            while (reader.Read())
            {
                lista.Add((
                    reader.GetInt32("id_turno"),
                    reader.GetInt32("socio_id"),
                    reader.GetString("socio_nombre"),
                    reader.GetInt32("nutricionista_id"),
                    reader.GetString("nutricionista_nombre"),
                    reader.GetDateTime("fecha"),
                    LeerHora(reader, "hora"),
                    reader.GetString("estado")
                ));
            }

            return lista;
        }

        public IEnumerable<(int Id, int SocioId, string SocioNombre, int NutricionistaId, string NutricionistaNombre, DateTime Fecha, TimeSpan Hora, string Estado)> ObtenerPorNutricionista(int nutricionistaId)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_obtener_turnos_nutricionista", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_nutricionista_id", nutricionistaId);

            using var reader = command.ExecuteReader();

            var lista = new List<(int, int, string, int, string, DateTime, TimeSpan, string)>();
            while (reader.Read())
            {
                lista.Add((
                    reader.GetInt32("id_turno"),
                    reader.GetInt32("socio_id"),
                    reader.GetString("socio_nombre"),
                    reader.GetInt32("nutricionista_id"),
                    reader.GetString("nutricionista_nombre"),
                    reader.GetDateTime("fecha"),
                    LeerHora(reader, "hora"),
                    reader.GetString("estado")
                ));
            }

            return lista;
        }

        public IEnumerable<(int Id, int SocioId, string SocioNombre, int NutricionistaId, string NutricionistaNombre, DateTime Fecha, TimeSpan Hora, string Estado)> ObtenerPorFecha(DateTime fecha)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_obtener_turnos_por_fecha", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_fecha", fecha.Date);

            using var reader = command.ExecuteReader();

            var lista = new List<(int, int, string, int, string, DateTime, TimeSpan, string)>();
            while (reader.Read())
            {
                lista.Add((
                    reader.GetInt32("id_turno"),
                    reader.GetInt32("socio_id"),
                    reader.GetString("socio_nombre"),
                    reader.GetInt32("nutricionista_id"),
                    reader.GetString("nutricionista_nombre"),
                    reader.GetDateTime("fecha"),
                    LeerHora(reader, "hora"),
                    reader.GetString("estado")
                ));
            }

            return lista;
        }

        /// <summary>
        /// Franjas estándar del consultorio menos las ya reservadas en turnos_nutricion.
        /// </summary>
        public IEnumerable<TimeSpan> ObtenerHorariosDisponibles(DateTime fecha, int nutricionistaId)
        {
            if (fecha.Date < DateTime.Today)
            {
                return Array.Empty<TimeSpan>();
            }

            try
            {
                var ocupados = ConsultarHorariosOcupados(fecha, nutricionistaId)
                    .Select(NormalizarHora)
                    .ToHashSet();
                return FranjasNutricion.Where(hora => !ocupados.Contains(NormalizarHora(hora)));
            }
            catch
            {
                return FranjasNutricion.ToArray();
            }
        }

        public (DateTime? Fecha, IReadOnlyList<TimeSpan> Horarios) BuscarProximoCupo(
            DateTime desde,
            int nutricionistaId,
            int maxDias = 90)
        {
            var inicio = desde.Date < DateTime.Today ? DateTime.Today : desde.Date;

            for (var dia = 0; dia <= maxDias; dia++)
            {
                var fecha = inicio.AddDays(dia);
                var horarios = ObtenerHorariosDisponibles(fecha, nutricionistaId).ToList();
                if (horarios.Count > 0)
                {
                    return (fecha, horarios);
                }
            }

            return (null, Array.Empty<TimeSpan>());
        }

        private static TimeSpan NormalizarHora(TimeSpan hora) => new(hora.Hours, hora.Minutes, 0);

        private IEnumerable<TimeSpan> ConsultarHorariosOcupados(DateTime fecha, int nutricionistaId)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand(
                """
                SELECT TIME_FORMAT(hora, '%H:%i:%s') AS hora_texto
                FROM turnos_nutricion
                WHERE nutricionista_id = @nutricionista_id
                  AND fecha = @fecha
                  AND estado <> 'CANCELADO'
                ORDER BY hora ASC
                """,
                connection);
            command.Parameters.AddWithValue("@nutricionista_id", nutricionistaId);
            command.Parameters.AddWithValue("@fecha", fecha.Date);

            using var reader = command.ExecuteReader();

            var lista = new List<TimeSpan>();
            while (reader.Read())
            {
                var hora = ParseHoraTexto(ObtenerTextoColumna(reader, "hora_texto"));
                if (hora != TimeSpan.Zero)
                {
                    lista.Add(hora);
                }
            }

            return lista;
        }

        public bool ExisteConflictoHorario(int nutricionistaId, DateTime fecha, TimeSpan hora, int? excluirTurnoId = null)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand(
                """
                SELECT COUNT(*) FROM turnos_nutricion
                WHERE nutricionista_id = @nutricionista_id
                  AND fecha = @fecha
                  AND hora = @hora
                  AND estado <> 'CANCELADO'
                  AND (@excluir IS NULL OR id_turno <> @excluir)
                """,
                connection);
            command.Parameters.AddWithValue("@nutricionista_id", nutricionistaId);
            command.Parameters.AddWithValue("@fecha", fecha.Date);
            command.Parameters.AddWithValue("@hora", hora);
            command.Parameters.AddWithValue("@excluir", excluirTurnoId.HasValue ? excluirTurnoId.Value : DBNull.Value);

            var count = command.ExecuteScalar();
            if (count is null)
            {
                return false;
            }

            return Convert.ToInt64(count, CultureInfo.InvariantCulture) > 0;
        }

        private static string? ObtenerTextoColumna(MySqlDataReader reader, string columna)
        {
            var ordinal = reader.GetOrdinal(columna);
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            var valor = reader.GetValue(ordinal);
            return valor switch
            {
                string s => s,
                TimeSpan ts => ts.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture),
                DateTime dt => dt.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                _ => Convert.ToString(valor, CultureInfo.InvariantCulture)
            };
        }

        private static TimeSpan ParseHoraTexto(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return TimeSpan.Zero;
            }

            var normalizado = texto.Trim();
            if (TimeSpan.TryParseExact(normalizado, FormatosHora, CultureInfo.InvariantCulture, out var hora))
            {
                return hora;
            }

            if (TimeSpan.TryParse(normalizado, CultureInfo.InvariantCulture, out hora))
            {
                return hora;
            }

            return TimeSpan.Zero;
        }

        private static TimeSpan LeerHora(MySqlDataReader reader, string columna)
        {
            var ordinal = reader.GetOrdinal(columna);
            if (reader.IsDBNull(ordinal))
            {
                return TimeSpan.Zero;
            }

            if (reader.GetFieldType(ordinal) == typeof(TimeSpan))
            {
                return reader.GetTimeSpan(ordinal);
            }

            return ParseHoraTexto(ObtenerTextoColumna(reader, columna));
        }

        public IEnumerable<(int Id, int SocioId, string SocioNombre, DateTime Fecha, TimeSpan Hora)> ObtenerDisponibles(DateTime fecha, int nutricionistaId)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_obtener_turnos_disponibles", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_fecha", fecha.Date);
            command.Parameters.AddWithValue("@p_nutricionista_id", nutricionistaId);

            using var reader = command.ExecuteReader();

            var lista = new List<(int, int, string, DateTime, TimeSpan)>();
            while (reader.Read())
            {
                lista.Add((
                    reader.GetInt32("id_turno"),
                    reader.GetInt32("socio_id"),
                    reader.GetString("socio_nombre"),
                    reader.GetDateTime("fecha"),
                    LeerHora(reader, "hora")
                ));
            }

            return lista;
        }

        public bool Crear(int socioId, int nutricionistaId, DateTime fecha, TimeSpan hora, string estado, out int turnoId)
        {
            turnoId = 0;
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_crear_turno_nutricion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_socio_id", socioId);
            command.Parameters.AddWithValue("@p_nutricionista_id", nutricionistaId);
            command.Parameters.AddWithValue("@p_fecha", fecha.Date);
            command.Parameters.AddWithValue("@p_hora", hora);
            command.Parameters.AddWithValue("@p_estado", estado);

            var outputParam = new MySqlParameter("@p_turno_id", MySqlDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputParam);

            try
            {
                command.ExecuteNonQuery();
                if (int.TryParse(outputParam.Value?.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                {
                    turnoId = id;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool Actualizar(int turnoId, DateTime fecha, TimeSpan hora, string estado)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_actualizar_turno_nutricion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_id_turno", turnoId);
            command.Parameters.AddWithValue("@p_fecha", fecha.Date);
            command.Parameters.AddWithValue("@p_hora", hora);
            command.Parameters.AddWithValue("@p_estado", estado);

            try
            {
                return command.ExecuteNonQuery() >= 1;
            }
            catch
            {
                return false;
            }
        }

        public bool Confirmar(int turnoId)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_confirmar_turno_nutricion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_id_turno", turnoId);

            try
            {
                return command.ExecuteNonQuery() >= 1;
            }
            catch
            {
                return false;
            }
        }

        public bool Cancelar(int turnoId)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_cancelar_turno_nutricion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_id_turno", turnoId);

            try
            {
                return command.ExecuteNonQuery() >= 1;
            }
            catch
            {
                return false;
            }
        }

        public bool Eliminar(int turnoId)
        {
            using var connection = _conexionFactory.ObtenerConexion();
            connection.Open();

            using var command = new MySqlCommand("sp_eliminar_turno_nutricion", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@p_id_turno", turnoId);

            try
            {
                return command.ExecuteNonQuery() >= 1;
            }
            catch
            {
                return false;
            }
        }
    }
}
