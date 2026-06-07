using TP_ClubDeportivo.Data;
using TP_ClubDeportivo.Forms;

namespace TP_ClubDeportivo;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        while (true)
        {
            using (var configuracion = new FormConfiguracionConexion())
            {
                if (configuracion.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }

            using (var inicializacion = new FormInicializacionBaseDatos())
            {
                if (inicializacion.ShowDialog() == DialogResult.OK)
                {
                    break;
                }
            }
        }

        Application.Run(new FormLogin());
    }
}
