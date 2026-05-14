namespace TP_ClubDeportivo.Models
{
    internal class Persona
    {
        public string DNI { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public string Telefono { get; set; }

        public string Direccion { get; set; }

        public Persona()
        {
        }

        public Persona(
            string dni,
            string nombre,
            string apellido,
            DateTime fechaNacimiento,
            string telefono,
            string direccion)
        {
            DNI = dni;
            Nombre = nombre;
            Apellido = apellido;
            FechaNacimiento = fechaNacimiento;
            Telefono = telefono;
            Direccion = direccion;
        }

        public virtual void ObtenerDatos()
        {
            Console.WriteLine($"Persona: {Nombre} {Apellido}");
        }
    }
}