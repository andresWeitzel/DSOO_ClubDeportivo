namespace TP_ClubDeportivo.Models
{
    internal class Carnet
    {
        public string Numero { get; set; }

        public DateTime FechaEmision { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public string Foto { get; set; }

        public void Emitir()
        {
            Console.WriteLine("Carnet emitido.");
        }

        public void Renovar()
        {
            Console.WriteLine("Carnet renovado.");
        }
    }
}