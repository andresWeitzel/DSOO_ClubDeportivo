namespace TP_ClubDeportivo.Models
{
    internal class Usuario
    {
        public int IdUsuario { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Rol { get; set; }

        public DateTime FechaRegistro { get; set; }

        public void Registrar()
        {
            Console.WriteLine("Usuario registrado.");
        }
    }
}