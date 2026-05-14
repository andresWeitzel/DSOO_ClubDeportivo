namespace TP_ClubDeportivo.Models
{
    internal class Pago
    {
        public string IdPago { get; set; }

        public double Monto { get; set; }

        public DateTime FechaPago { get; set; }

        public string MedioPago { get; set; }

        public void Registrar()
        {
            Console.WriteLine("Pago registrado.");
        }
    }
}