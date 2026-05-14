namespace TP_ClubDeportivo.Models
{
    internal class FichaMedica
    {
        public string IdFicha { get; set; }

        public string Antecedentes { get; set; }

        public string Alergias { get; set; }

        public string Medicacion { get; set; }

        public string CargaActividadPermitida { get; set; }

        public void Actualizar()
        {
            Console.WriteLine("Ficha médica actualizada.");
        }
    }
}