namespace TP_ClubDeportivo.Models
{
    internal class Actividad
    {
        public string IdActividad { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public int CupoMaximo { get; set; }

        public HorarioActividad Horario { get; set; }

        public void InscribirSocio()
        {
            Console.WriteLine("Socio inscripto en la actividad.");
        }
    }
}