namespace TP_ClubDeportivo.Models
{
    internal class Visitante : Persona
    {
        public DateTime FechaIngreso { get; set; }

        public void RegistrarIngreso()
        {
            Console.WriteLine("Ingreso registrado.");
        }
    }
}