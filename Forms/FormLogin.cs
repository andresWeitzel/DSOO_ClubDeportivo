using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TP_ClubDeportivo;
using TP_ClubDeportivo.DAO;
using TP_ClubDeportivo.Data;

namespace TP_ClubDeportivo.Forms
{
    public class FormLogin : Form
    {
        private const int AnchoCampos = 380;
        private const int AltoCampo = 32;

        private static readonly string ArchivoUsuarioRecordado = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TP_ClubDeportivo",
            "usuario.txt");

        private readonly TextBox txtUsername;
        private readonly TextBox txtPassword;
        private readonly Button btnLogin;
        private readonly Button btnTestConexion;
        private readonly Button btnTogglePassword;
        private readonly CheckBox chkRecordarUsuario;
        private readonly Label lblError;

        public FormLogin()
        {
            Text = "Club Deportivo";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            ClientSize = new Size(820, 560);
            MinimumSize = new Size(820, 560);
            BackColor = UiTheme.Fondo;
            Font = UiTheme.FuenteNormal;
            DoubleBuffered = true;
            AutoScaleMode = AutoScaleMode.Font;

            var panelIzquierdo = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = UiTheme.Primario
            };
            panelIzquierdo.Paint += PanelIzquierdo_Paint;

            var panelPieSidebar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                BackColor = Color.Transparent
            };
            panelPieSidebar.Controls.Add(new Label
            {
                Text = "v1.0 — TP Desarrollo de Sistemas",
                ForeColor = Color.FromArgb(180, 210, 245),
                Font = new Font("Segoe UI", 8.5F),
                AutoSize = true,
                Location = new Point(32, 16)
            });

            var lblMarca = new Label
            {
                Text = "Club Deportivo",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(32, 48)
            };

            var lblTagline = new Label
            {
                Text = "Sistema de gestión integral",
                ForeColor = Color.FromArgb(220, 235, 255),
                Font = new Font("Segoe UI", 11F),
                AutoSize = true,
                Location = new Point(32, 96)
            };

            var lblModulos = new Label
            {
                Text = "Socios · Cuotas · Personal · Nutrición",
                ForeColor = Color.FromArgb(200, 220, 245),
                Font = new Font("Segoe UI", 9.5F),
                AutoSize = true,
                Location = new Point(32, 126)
            };

            panelIzquierdo.Controls.AddRange([lblMarca, lblTagline, lblModulos, panelPieSidebar]);

            var panelDerecho = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Fondo };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(48, 40, 48, 32),
                BackColor = UiTheme.Fondo,
                ColumnCount = 1,
                AutoSize = false
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            AgregarFilaAuto(layout, new Label
            {
                Text = "Iniciar sesión",
                Font = UiTheme.FuenteTitulo,
                ForeColor = UiTheme.Texto,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 8)
            });

            AgregarFilaAuto(layout, new Label
            {
                Text = "Ingrese sus credenciales para acceder al sistema.",
                Font = UiTheme.FuenteSubtitulo,
                ForeColor = UiTheme.TextoSecundario,
                AutoSize = true,
                MaximumSize = new Size(AnchoCampos, 0),
                Margin = new Padding(0, 0, 0, 20)
            });

            txtUsername = new TextBox
            {
                Height = AltoCampo,
                PlaceholderText = "Ej: admin"
            };
            UiTheme.AplicarCampo(txtUsername);
            AgregarFilaAuto(layout, UiTheme.CrearCampoVertical("Usuario", txtUsername));

            txtPassword = new TextBox
            {
                Height = AltoCampo,
                UseSystemPasswordChar = true,
                PlaceholderText = "Ingrese su contraseña"
            };
            UiTheme.AplicarCampo(txtPassword);

            btnTogglePassword = new Button { Text = "Ver", TabStop = false };
            UiTheme.AplicarBotonSecundario(btnTogglePassword);
            btnTogglePassword.MinimumSize = new Size(92, AltoCampo);
            btnTogglePassword.Click += BtnTogglePassword_Click;
            AgregarFilaAuto(layout, UiTheme.CrearCampoVertical("Contraseña", txtPassword, btnTogglePassword));

            chkRecordarUsuario = new CheckBox
            {
                Text = "Recordar usuario",
                AutoSize = true,
                ForeColor = UiTheme.TextoSecundario,
                Margin = new Padding(0, 8, 0, 0)
            };
            AgregarFilaAuto(layout, chkRecordarUsuario);

            lblError = new Label
            {
                ForeColor = UiTheme.Error,
                AutoSize = true,
                MaximumSize = new Size(AnchoCampos, 0),
                Visible = false,
                Margin = new Padding(0, 10, 0, 0)
            };
            AgregarFilaAuto(layout, lblError);

            var panelBotones = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 3,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 16, 0, 0),
                MaximumSize = new Size(AnchoCampos, 0)
            };
            panelBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelBotones.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            panelBotones.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            panelBotones.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

            btnLogin = new Button
            {
                Text = "Ingresar",
                Dock = DockStyle.Fill
            };
            UiTheme.AplicarBotonPrimario(btnLogin);
            btnLogin.Click += BtnLogin_Click;

            var panelAcciones = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0)
            };
            panelAcciones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            panelAcciones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            panelAcciones.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

            btnTestConexion = new Button
            {
                Text = "Probar conexión",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 8, 0)
            };
            UiTheme.AplicarBotonSecundario(btnTestConexion);
            btnTestConexion.Click += BtnTestConexion_Click;

            var btnCambiarConexion = new Button
            {
                Text = "Cambiar MySQL",
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };
            UiTheme.AplicarBotonSecundario(btnCambiarConexion);
            btnCambiarConexion.Click += BtnCambiarConexion_Click;

            panelAcciones.Controls.Add(btnTestConexion, 0, 0);
            panelAcciones.Controls.Add(btnCambiarConexion, 1, 0);
            panelBotones.Controls.Add(btnLogin, 0, 0);
            panelBotones.Controls.Add(panelAcciones, 0, 2);
            AgregarFilaAuto(layout, panelBotones);

            AgregarFilaEspaciador(layout);

            panelDerecho.Controls.Add(layout);

            Controls.Add(panelDerecho);
            Controls.Add(panelIzquierdo);

            AcceptButton = btnLogin;
            txtUsername.KeyDown += Campo_KeyDown;
            txtPassword.KeyDown += Campo_KeyDown;
            Shown += OnShownInicial;
        }

        private static void AgregarFilaAuto(TableLayoutPanel tabla, Control control)
        {
            var fila = tabla.RowCount;
            tabla.RowCount++;
            tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            control.Dock = DockStyle.Top;
            tabla.Controls.Add(control, 0, fila);
        }

        private static void AgregarFilaEspaciador(TableLayoutPanel tabla)
        {
            var fila = tabla.RowCount;
            tabla.RowCount++;
            tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tabla.Controls.Add(new Panel { Dock = DockStyle.Fill, Height = 1 }, 0, fila);
        }

        private void OnShownInicial(object? sender, EventArgs e)
        {
            if (!InicializadorBaseDatos.BaseDeDatosLista())
            {
                var reintentar = MessageBox.Show(
                    "La base de datos «db_club_deportivo» no está disponible.\n\n" +
                    "¿Desea crearla e inicializarla ahora?",
                    "Club Deportivo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (reintentar == DialogResult.Yes)
                {
                    using var inicializacion = new FormInicializacionBaseDatos();
                    if (inicializacion.ShowDialog() != DialogResult.OK)
                    {
                        Close();
                        return;
                    }
                }
                else
                {
                    MostrarError(
                        "Base de datos no disponible.\n" +
                        "Use «Probar conexión» o reinicie la aplicación.");
                }
            }

            CargarUsuarioRecordado();
            txtUsername.Focus();
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                txtUsername.Select();
            }
            else
            {
                txtPassword.Focus();
            }
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
            e.Graphics.FillEllipse(circulo, panel.Width - 120, panel.Height - 140, 200, 200);
            e.Graphics.FillEllipse(circulo, -60, 200, 160, 160);
        }

        private void Campo_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                BtnLogin_Click(btnLogin, EventArgs.Empty);
            }
        }

        private void BtnTogglePassword_Click(object? sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            btnTogglePassword.Text = txtPassword.UseSystemPasswordChar ? "Ver" : "Ocultar";
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            OcultarError();

            var usuario = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrEmpty(usuario))
            {
                MostrarError("Ingrese su nombre de usuario.");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MostrarError("Ingrese su contraseña.");
                txtPassword.Focus();
                return;
            }

            EstablecerCargando(true);

            try
            {
                var resultado = new UsuarioDAO().Login(usuario, password);
                if (resultado is null)
                {
                    MostrarError("Usuario o contraseña incorrectos. Verifique sus datos e intente nuevamente.");
                    txtPassword.SelectAll();
                    txtPassword.Focus();
                    return;
                }

                GuardarUsuarioRecordado(usuario);

                Sesion.Iniciar(resultado);

                if (!Permisos.PuedeIngresarAlSistema())
                {
                    var rol = resultado.Rol;
                    using var accesoDenegado = new FormAccesoDenegado(
                        $"El rol «{rol}» no puede usar la aplicación de gestión del club.\n\n" +
                        "Esta versión está destinada a personal interno: Administrador, Empleado, Profesor o Nutricionista.\n\n" +
                        "Los usuarios Socio y Visitante no tienen acceso a este módulo.");
                    accesoDenegado.ShowDialog();
                    Sesion.Cerrar();
                    txtPassword.Clear();
                    txtPassword.Focus();
                    return;
                }

                Hide();
                using var principal = new FormPrincipal();
                principal.ShowDialog();
                Sesion.Cerrar();

                txtPassword.Clear();
                OcultarError();
                Show();
                txtPassword.Focus();
            }
            catch (Exception ex)
            {
                var datos = Conexion.ObtenerDatosInstalacion();
                var resumenDatos = datos is not null
                    ? "\n\nDatos de conexión:\n" + datos.ResumenTexto
                    : string.Empty;
                MostrarError($"No se pudo conectar al sistema.\n{ex.Message}{resumenDatos}");
            }
            finally
            {
                EstablecerCargando(false);
            }
        }

        private void BtnCambiarConexion_Click(object? sender, EventArgs e)
        {
            using var configuracion = new FormConfiguracionConexion();
            if (configuracion.ShowDialog() == DialogResult.OK)
            {
                OcultarError();
                MostrarMensaje("Datos de conexión actualizados. Pruebe la conexión antes de ingresar.", true);
            }
        }

        private void BtnTestConexion_Click(object? sender, EventArgs e)
        {
            OcultarError();
            EstablecerCargando(true);

            try
            {
                var datos = Conexion.ObtenerDatosInstalacion();
                var resumenDatos = datos?.ResumenTexto ?? "No hay datos de conexión configurados.";

                var ok = Conexion.ProbarConexionActual(out var detalle);
                if (ok)
                {
                    MessageBox.Show(
                        "Conexión a la base de datos establecida correctamente.\n\n" +
                        "Datos configurados:\n" + resumenDatos,
                        "Club Deportivo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    MostrarMensaje("Conexión OK. Puede iniciar sesión.", true);
                }
                else
                {
                    MostrarError(
                        "No se pudo conectar al servidor.\n\n" +
                        "Datos configurados:\n" + resumenDatos + "\n\n" +
                        "Detalle: " + detalle);
                }
            }
            catch (Exception ex)
            {
                var datos = Conexion.ObtenerDatosInstalacion();
                var resumenDatos = datos?.ResumenTexto ?? "No hay datos de conexión configurados.";
                MostrarError($"Error de conexión: {ex.Message}\n\nDatos configurados:\n{resumenDatos}");
            }
            finally
            {
                EstablecerCargando(false);
            }
        }

        private void EstablecerCargando(bool cargando)
        {
            btnLogin.Enabled = !cargando;
            btnTestConexion.Enabled = !cargando;
            txtUsername.Enabled = !cargando;
            txtPassword.Enabled = !cargando;
            btnTogglePassword.Enabled = !cargando;
            chkRecordarUsuario.Enabled = !cargando;
            btnLogin.Text = cargando ? "Ingresando…" : "Ingresar";
            Cursor = cargando ? Cursors.WaitCursor : Cursors.Default;
        }

        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            lblError.ForeColor = UiTheme.Error;
            lblError.Visible = true;
        }

        private void MostrarMensaje(string mensaje, bool exito)
        {
            lblError.Text = mensaje;
            lblError.ForeColor = exito ? UiTheme.Exito : UiTheme.Error;
            lblError.Visible = true;
        }

        private void OcultarError() => lblError.Visible = false;

        private void CargarUsuarioRecordado()
        {
            try
            {
                if (!File.Exists(ArchivoUsuarioRecordado))
                {
                    return;
                }

                var usuario = File.ReadAllText(ArchivoUsuarioRecordado).Trim();
                if (!string.IsNullOrEmpty(usuario))
                {
                    txtUsername.Text = usuario;
                    chkRecordarUsuario.Checked = true;
                }
            }
            catch
            {
                // Ignorar errores de lectura local
            }
        }

        private void GuardarUsuarioRecordado(string usuario)
        {
            try
            {
                if (chkRecordarUsuario.Checked)
                {
                    var dir = Path.GetDirectoryName(ArchivoUsuarioRecordado)!;
                    Directory.CreateDirectory(dir);
                    File.WriteAllText(ArchivoUsuarioRecordado, usuario);
                }
                else if (File.Exists(ArchivoUsuarioRecordado))
                {
                    File.Delete(ArchivoUsuarioRecordado);
                }
            }
            catch
            {
                // Ignorar errores de escritura local
            }
        }
    }
}
