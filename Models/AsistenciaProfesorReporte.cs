namespace TP_ClubDeportivo.Models
{
    /// <summary>
    /// Proyección para reporte RF-17 (asistencia de profesores).
    /// </summary>
    internal class AsistenciaProfesorReporte
    {
        public int IdProfesor { get; set; }

        public string ProfesorNombre { get; set; } = string.Empty;

        public string Especialidad { get; set; } = string.Empty;

        public int TotalRegistros { get; set; }

        public int Asistencias { get; set; }

        public int Inasistencias { get; set; }

        public double PorcentajeAsistencia { get; set; }
    }
}
