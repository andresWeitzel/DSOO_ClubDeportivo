namespace TP_ClubDeportivo.Data
{
    public sealed class ProgresoInicializacionBaseDatos
    {
        public int Paso { get; init; }
        public int Total { get; init; }
        public string NombreArchivo { get; init; } = string.Empty;
        public string MensajeAmigable { get; init; } = string.Empty;
        public bool EsMensajeSistema { get; init; }
    }

    public static class MensajesScriptsBaseDatos
    {
        public static string ObtenerMensajeAmigable(string nombreArchivo) =>
            nombreArchivo.ToLowerInvariant() switch
            {
                "01_ddl.sql" => "Creando tablas y estructura del sistema…",
                "02_dml.sql" => "Cargando datos iniciales del club deportivo…",
                "03_sp_socios.sql" => "Preparando la gestión de socios…",
                "04_sp_cuotas.sql" => "Configurando el módulo de cuotas…",
                "05_sp_pagos.sql" => "Configurando el registro de pagos…",
                "06_sp_carnets.sql" => "Preparando la emisión de carnets…",
                "07_sp_visitantes.sql" => "Configurando el control de visitantes…",
                "08_sp_fichas_medicas.sql" => "Preparando fichas médicas…",
                "09_sp_reportes.sql" => "Configurando reportes del sistema…",
                "10_sp_usuarios.sql" => "Preparando usuarios y accesos…",
                "11_sp_actividades.sql" => "Configurando actividades deportivas…",
                "12_sp_profesores.sql" => "Preparando la gestión de profesores…",
                "13_sp_horarios_actividad.sql" => "Configurando horarios de actividades…",
                "14_sp_asistencias.sql" => "Preparando el control de asistencias…",
                "15_sp_rutinas.sql" => "Configurando rutinas de entrenamiento…",
                "16_sp_fichas_medicas.sql" => "Completando procedimientos de fichas médicas…",
                "17_sp_nutricionistas.sql" => "Preparando la gestión de nutricionistas…",
                "18_sp_turnos_nutricion.sql" => "Configurando turnos de nutrición…",
                "19_sp_liquidaciones.sql" => "Finalizando liquidaciones y cierre del sistema…",
                _ => "Cargando componentes en la base de datos…"
            };
    }
}
