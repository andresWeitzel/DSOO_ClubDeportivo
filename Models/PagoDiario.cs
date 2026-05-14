namespace TP_ClubDeportivo.Models
{
    internal class PagoDiario
    {
        public DateTime FechaIngreso { get; set; }

        public void EmitirComprobante()
        {
            Console.WriteLine("Comprobante emitido.");
        }
    }
}