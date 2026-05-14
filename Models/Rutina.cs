namespace TP_ClubDeportivo.Models
{
    internal class Rutina
    {
        public string IdRutina { get; set; }

        public DateTime FechaCreacion { get; set; }

        public List<string> Ejercicios { get; set; }

        public string Observaciones { get; set; }

        public void Modificar()
        {
            Console.WriteLine("Rutina modificada.");
        }
    }
}