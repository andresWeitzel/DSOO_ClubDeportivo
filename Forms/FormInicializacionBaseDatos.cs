using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TP_ClubDeportivo.Data;

namespace TP_ClubDeportivo.Forms
{
    public class FormInicializacionBaseDatos : Form
    {
        private readonly Label lblPaso;
        private readonly Label lblEstado;
        private readonly Label lblDetalle;
        private readonly ProgressBar progressBar;
        private readonly ListBox lstPasos;
        private int pasoAnterior;

        public FormInicializacionBaseDatos()
        {
            Text = "Club Deportivo — Base de datos";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            ClientSize = new Size(780, 480);
            BackColor = UiTheme.Fondo;
            Font = UiTheme.FuenteNormal;
            DoubleBuffered = true;

            var panelIzquierdo = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = UiTheme.Primario
            };
            panelIzquierdo.Paint += PanelIzquierdo_Paint;

            panelIzquierdo.Controls.AddRange([
                new Label
                {
                    Text = "Club Deportivo",
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(28, 44)
                },
                new Label
                {
                    Text = "Estamos preparando la base de datos para que pueda usar el sistema con normalidad.",
                    ForeColor = Color.FromArgb(220, 235, 255),
                    Font = new Font("Segoe UI", 10F),
                    Size = new Size(210, 120),
                    Location = new Point(28, 96)
                }
            ]);

            var panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(36, 32, 36, 24),
                BackColor = UiTheme.Fondo
            };

            var lblTitulo = new Label
            {
                Text = "Configurando la base de datos",
                Font = UiTheme.FuenteTitulo,
                ForeColor = UiTheme.Texto,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var lblSubtitulo = new Label
            {
                Text = "Cargando tablas, datos iniciales y procedimientos del club deportivo.",
                Font = UiTheme.FuenteSubtitulo,
                ForeColor = UiTheme.TextoSecundario,
                AutoSize = true,
                MaximumSize = new Size(440, 0),
                Location = new Point(0, 40)
            };

            lblPaso = new Label
            {
                Text = "Iniciando…",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = UiTheme.Primario,
                AutoSize = true,
                Location = new Point(0, 88)
            };

            progressBar = new ProgressBar
            {
                Location = new Point(0, 112),
                Size = new Size(440, 10),
                Style = ProgressBarStyle.Continuous,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            lblEstado = new Label
            {
                Text = "Verificando instalación…",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = UiTheme.Texto,
                AutoSize = true,
                MaximumSize = new Size(440, 0),
                Location = new Point(0, 136)
            };

            lblDetalle = new Label
            {
                Text = "Este proceso puede tardar unos segundos. No cierre la aplicación.",
                ForeColor = UiTheme.TextoSecundario,
                AutoSize = true,
                MaximumSize = new Size(440, 0),
                Location = new Point(0, 168)
            };

            lstPasos = new ListBox
            {
                Location = new Point(0, 204),
                Size = new Size(440, 200),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = UiTheme.TextoSecundario,
                Font = new Font("Segoe UI", 9F),
                IntegralHeight = false,
                ItemHeight = 22
            };

            panelContenido.Controls.AddRange([
                lblTitulo, lblSubtitulo, lblPaso, progressBar, lblEstado, lblDetalle, lstPasos
            ]);

            Controls.Add(panelContenido);
            Controls.Add(panelIzquierdo);

            Shown += OnShown;
        }

        private async void OnShown(object? sender, EventArgs e)
        {
            try
            {
                var resultado = await Task.Run(() =>
                    InicializadorBaseDatos.EjecutarSiEsNecesario(ActualizarProgreso));

                if (!resultado.Exito)
                {
                    MessageBox.Show(
                        "No se pudo preparar la base de datos.\n\n" +
                        resultado.Mensaje + "\n\n" +
                        "Verifique que MySQL/MariaDB esté en ejecución y que el usuario tenga permisos para crear bases y procedures.",
                        "Club Deportivo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    DialogResult = DialogResult.Cancel;
                    Close();
                    return;
                }

                ActualizarProgreso(new ProgresoInicializacionBaseDatos
                {
                    MensajeAmigable = resultado.SeEjecutaronScripts
                        ? "Base de datos creada e inicializada correctamente."
                        : "Base de datos verificada. Todo listo.",
                    EsMensajeSistema = true
                });
                progressBar.Value = progressBar.Maximum;
                lblDetalle.Text = resultado.Mensaje;
                lblDetalle.ForeColor = UiTheme.Exito;

                await Task.Delay(450);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al inicializar la base de datos:\n\n" + ex.Message,
                    "Club Deportivo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void ActualizarProgreso(ProgresoInicializacionBaseDatos progreso)
        {
            if (InvokeRequired)
            {
                BeginInvoke(ActualizarProgreso, progreso);
                return;
            }

            lblEstado.Text = progreso.MensajeAmigable;

            if (progreso.Total > 0)
            {
                progressBar.Maximum = progreso.Total;
                progressBar.Value = Math.Min(progreso.Paso, progreso.Total);
                lblPaso.Text = $"Paso {progreso.Paso} de {progreso.Total}";
                lblDetalle.Text = string.IsNullOrWhiteSpace(progreso.NombreArchivo)
                    ? "Cargando datos en el sistema…"
                    : $"Procesando: {progreso.NombreArchivo}";

                if (pasoAnterior > 0 && pasoAnterior < progreso.Paso)
                {
                    MarcarPasoCompletado(pasoAnterior);
                }

                AgregarPasoActual(progreso);
                pasoAnterior = progreso.Paso;
            }
            else if (progreso.EsMensajeSistema)
            {
                lblPaso.Text = "Preparando…";
            }

            Refresh();
        }

        private void MarcarPasoCompletado(int paso)
        {
            for (var i = lstPasos.Items.Count - 1; i >= 0; i--)
            {
                var texto = lstPasos.Items[i]?.ToString();
                if (texto is not null && texto.StartsWith("▶ ", StringComparison.Ordinal))
                {
                    lstPasos.Items[i] = texto.Replace("▶ ", "✓ ", StringComparison.Ordinal);
                    break;
                }
            }
        }

        private void AgregarPasoActual(ProgresoInicializacionBaseDatos progreso)
        {
            var itemActual = $"▶ {progreso.Paso}. {progreso.MensajeAmigable.TrimEnd('…', '.')}";
            var ultimo = lstPasos.Items.Count > 0 ? lstPasos.Items[lstPasos.Items.Count - 1]?.ToString() : null;
            if (ultimo == itemActual)
            {
                return;
            }

            if (ultimo is not null && ultimo.StartsWith("▶ ", StringComparison.Ordinal))
            {
                lstPasos.Items[lstPasos.Items.Count - 1] = ultimo.Replace("▶ ", "✓ ", StringComparison.Ordinal);
            }

            lstPasos.Items.Add(itemActual);
            lstPasos.TopIndex = Math.Max(0, lstPasos.Items.Count - 1);
        }

        private static void PanelIzquierdo_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel)
            {
                return;
            }

            using var brush = new LinearGradientBrush(
                panel.ClientRectangle,
                UiTheme.Primario,
                UiTheme.PrimarioOscuro,
                LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, panel.ClientRectangle);

            using var circulo = new SolidBrush(Color.FromArgb(30, 255, 255, 255));
            e.Graphics.FillEllipse(circulo, panel.Width - 100, panel.Height - 120, 180, 180);
            e.Graphics.FillEllipse(circulo, -50, 180, 140, 140);
        }
    }
}
