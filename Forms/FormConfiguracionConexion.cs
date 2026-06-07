using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TP_ClubDeportivo.Data;

namespace TP_ClubDeportivo.Forms
{
    public class FormConfiguracionConexion : Form
    {
        private const int AnchoCampos = 360;
        private const int AltoCampo = 32;

        private readonly TextBox txtServidor;
        private readonly TextBox txtPuerto;
        private readonly TextBox txtUsuario;
        private readonly TextBox txtClave;
        private readonly Button btnContinuar;
        private readonly Label lblMensaje;

        public FormConfiguracionConexion()
        {
            Text = "Datos de instalación MySQL";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(780, 620);
            MinimumSize = new Size(780, 620);
            BackColor = UiTheme.Fondo;
            Font = UiTheme.FuenteNormal;
            AutoScaleMode = AutoScaleMode.Font;

            var panelIzquierdo = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
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
                    Text = "Configure el acceso al motor MySQL/MariaDB antes de ingresar al sistema.",
                    ForeColor = Color.FromArgb(220, 235, 255),
                    Font = new Font("Segoe UI", 10F),
                    Size = new Size(220, 100),
                    Location = new Point(28, 96)
                }
            ]);

            var panelDerecho = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Fondo };

            var panelBotones = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 72,
                Padding = new Padding(36, 0, 36, 20),
                BackColor = UiTheme.Fondo,
                ColumnCount = 2,
                RowCount = 1
            };
            panelBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            panelBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            panelBotones.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));

            btnContinuar = new Button
            {
                Text = "Continuar al login",
                Dock = DockStyle.Fill,
                Margin = new Padding(8, 0, 0, 0)
            };
            UiTheme.AplicarBotonPrimario(btnContinuar);
            btnContinuar.Click += BtnContinuar_Click;

            var btnRestablecer = new Button
            {
                Text = "Restablecer valores",
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };
            UiTheme.AplicarBotonSecundario(btnRestablecer);
            btnRestablecer.Click += (_, _) => RestablecerValores();

            panelBotones.Controls.Add(btnRestablecer, 0, 0);
            panelBotones.Controls.Add(btnContinuar, 1, 0);

            var panelCampos = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(36, 24, 36, 8),
                BackColor = UiTheme.Fondo,
                ColumnCount = 1,
                AutoScroll = false
            };
            panelCampos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            AgregarFila(panelCampos, new Label
            {
                Text = "Conexión a la base de datos",
                Font = UiTheme.FuenteTitulo,
                ForeColor = UiTheme.Texto,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 4)
            }, 36);

            AgregarFila(panelCampos, new Label
            {
                Text = "Ingrese los datos del servidor MySQL/MariaDB.",
                Font = UiTheme.FuenteSubtitulo,
                ForeColor = UiTheme.TextoSecundario,
                AutoSize = true,
                MaximumSize = new Size(AnchoCampos, 0),
                Margin = new Padding(0, 0, 0, 8)
            }, 28);

            AgregarFila(panelCampos, CrearBadgeBaseDatos(), 52);

            txtServidor = AgregarCampoEnTabla(panelCampos, "Servidor", "localhost");
            txtPuerto = AgregarCampoEnTabla(panelCampos, "Puerto", "3306");
            txtUsuario = AgregarCampoEnTabla(panelCampos, "Usuario", "root");
            txtClave = AgregarCampoEnTabla(
                panelCampos,
                "Contraseña",
                string.Empty,
                password: true,
                placeholder: "Opcional si no tiene clave");

            lblMensaje = new Label
            {
                ForeColor = UiTheme.Error,
                AutoSize = true,
                MaximumSize = new Size(AnchoCampos, 0),
                Visible = false,
                Margin = new Padding(0, 4, 0, 0)
            };
            AgregarFila(panelCampos, lblMensaje, 24);

            panelDerecho.Controls.Add(panelCampos);
            panelDerecho.Controls.Add(panelBotones);

            Controls.Add(panelDerecho);
            Controls.Add(panelIzquierdo);

            AcceptButton = btnContinuar;
            FormClosing += OnFormClosing;
            Shown += (_, _) => CargarDatosExistentes();
        }

        private static void AgregarFila(TableLayoutPanel tabla, Control control, int altoFila)
        {
            var fila = tabla.RowCount;
            tabla.RowCount++;
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, altoFila));
            control.Dock = DockStyle.Fill;
            tabla.Controls.Add(control, 0, fila);
        }

        private static TextBox AgregarCampoEnTabla(
            TableLayoutPanel tabla,
            string etiqueta,
            string valor,
            bool password = false,
            string? placeholder = null)
        {
            var contenedor = new Panel
            {
                Height = 54,
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };

            var lbl = new Label
            {
                Text = etiqueta,
                ForeColor = UiTheme.Texto,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var textBox = new TextBox
            {
                Location = new Point(0, 20),
                Size = new Size(AnchoCampos, AltoCampo),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                Text = valor
            };

            if (password)
            {
                textBox.UseSystemPasswordChar = true;
            }

            if (!string.IsNullOrEmpty(placeholder))
            {
                textBox.PlaceholderText = placeholder;
            }

            UiTheme.AplicarCampo(textBox);
            contenedor.Controls.AddRange([lbl, textBox]);
            AgregarFila(tabla, contenedor, 58);

            return textBox;
        }

        private static Panel CrearBadgeBaseDatos()
        {
            var contenedor = new Panel
            {
                Height = 48,
                BackColor = UiTheme.PrimarioClaro,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 4)
            };

            contenedor.Paint += (_, e) =>
            {
                var rect = contenedor.ClientRectangle;
                rect.Width -= 1;
                rect.Height -= 1;
                using var pen = new Pen(UiTheme.Primario, 1.5f);
                e.Graphics.DrawRectangle(pen, rect);
                using var barra = new SolidBrush(UiTheme.Primario);
                e.Graphics.FillRectangle(barra, 0, 0, 4, contenedor.Height);
            };

            contenedor.Controls.AddRange([
                new Label
                {
                    Text = "Base de datos del sistema",
                    ForeColor = UiTheme.TextoSecundario,
                    Font = new Font("Segoe UI", 8.5F),
                    AutoSize = true,
                    Location = new Point(12, 6)
                },
                new Label
                {
                    Text = Conexion.NombreBaseDatos,
                    ForeColor = UiTheme.Primario,
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(12, 24)
                },
                new Label
                {
                    Text = "Nombre fijo",
                    ForeColor = UiTheme.PrimarioOscuro,
                    Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                    AutoSize = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Location = new Point(contenedor.Width - 82, 8)
                }
            ]);

            return contenedor;
        }

        private void RestablecerValores()
        {
            txtServidor.Text = "localhost";
            txtPuerto.Text = "3306";
            txtUsuario.Text = "root";
            txtClave.Text = string.Empty;
            OcultarMensaje();
            txtServidor.Focus();
        }

        private void CargarDatosExistentes()
        {
            var datos = Conexion.ObtenerDatosInstalacion();
            if (datos is null)
            {
                RestablecerValores();
                return;
            }

            txtServidor.Text = datos.Servidor;
            txtPuerto.Text = datos.Puerto;
            txtUsuario.Text = datos.Usuario;
            txtClave.Text = datos.Clave;
            txtServidor.Focus();
        }

        private void BtnContinuar_Click(object? sender, EventArgs e)
        {
            OcultarMensaje();

            var servidor = txtServidor.Text.Trim();
            var puerto = txtPuerto.Text.Trim();
            var usuario = txtUsuario.Text.Trim();
            var clave = txtClave.Text;

            if (string.IsNullOrEmpty(servidor))
            {
                MostrarMensaje("Ingrese el servidor.");
                txtServidor.Focus();
                return;
            }

            if (string.IsNullOrEmpty(puerto))
            {
                MostrarMensaje("Ingrese el puerto.");
                txtPuerto.Focus();
                return;
            }

            if (!uint.TryParse(puerto, out _))
            {
                MostrarMensaje("El puerto debe ser un número válido.");
                txtPuerto.Focus();
                return;
            }

            if (string.IsNullOrEmpty(usuario))
            {
                MostrarMensaje("Ingrese el usuario.");
                txtUsuario.Focus();
                return;
            }

            var datos = new DatosInstalacionMySql(
                servidor,
                puerto,
                usuario,
                clave,
                Conexion.NombreBaseDatos);

            var confirmacion = MessageBox.Show(
                "Su ingreso:\n\n" + datos.ResumenTexto + "\n\n¿Los datos son correctos?",
                "AVISO DEL SISTEMA",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                MostrarMensaje("Revise los datos e intente nuevamente.");
                return;
            }

            Conexion.EstablecerConfiguracion(datos);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK || Conexion.EstaConfigurada)
            {
                return;
            }

            var salir = MessageBox.Show(
                "Debe configurar la conexión para usar el sistema.\n\n¿Desea salir de la aplicación?",
                "Club Deportivo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (salir == DialogResult.Yes)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }

            e.Cancel = true;
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
        }

        private void MostrarMensaje(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
        }

        private void OcultarMensaje() => lblMensaje.Visible = false;
    }
}
