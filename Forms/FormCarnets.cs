using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using TP_ClubDeportivo.DAO;
using TP_ClubDeportivo.Models;

namespace TP_ClubDeportivo.Forms
{
    public class FormCarnets : Form
    {
        private readonly TextBox txtDni;
        private readonly Label lblSocio;
        private readonly Label lblEstado;
        private readonly TextBox txtNumero;
        private readonly TextBox txtEmision;
        private readonly TextBox txtVencimiento;
        private readonly Button btnBuscar;
        private readonly Button btnLimpiar;
        private readonly Button btnRenovar;
        private readonly Button btnEmitir;
        private readonly Label lblMensaje;

        private readonly SocioDAO _socioDao = new();
        private readonly CarnetDAO _carnetDao = new();

        private Socio? _socioSeleccionado;
        private Carnet? _carnetActual;

        public FormCarnets()
        {
            Text = "Gestión de carnets (CU-04)";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(520, 420);
            MinimumSize = new Size(480, 400);
            Font = UiTheme.FuenteNormal;
            BackColor = UiTheme.Fondo;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var panelBusqueda = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                Padding = new Padding(12, 10, 12, 8),
                BackColor = UiTheme.Tarjeta
            };

            panelBusqueda.Controls.Add(new Label
            {
                Text = "DNI del socio:",
                Location = new Point(12, 14),
                AutoSize = true
            });

            txtDni = new TextBox
            {
                Location = new Point(110, 10),
                Width = 160,
                PlaceholderText = "Ej: 12345678"
            };
            UiTheme.AplicarCampo(txtDni);

            btnBuscar = new Button
            {
                Text = "Buscar",
                Location = new Point(280, 8),
                Size = new Size(90, 32)
            };
            UiTheme.AplicarBotonPrimario(btnBuscar);
            btnBuscar.Click += (_, _) => BuscarSocio();

            btnLimpiar = new Button
            {
                Text = "Limpiar",
                Location = new Point(378, 8),
                Size = new Size(80, 32)
            };
            UiTheme.AplicarBotonSecundario(btnLimpiar);
            btnLimpiar.Click += (_, _) => Limpiar();

            lblSocio = new Label
            {
                Location = new Point(12, 52),
                Size = new Size(440, 22),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = UiTheme.Primario
            };

            panelBusqueda.Controls.AddRange([txtDni, btnBuscar, btnLimpiar, lblSocio]);

            var grpCarnet = new GroupBox
            {
                Text = "Datos del carnet",
                Location = new Point(12, 100),
                Size = new Size(476, 200),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            lblEstado = new Label
            {
                Location = new Point(16, 32),
                Size = new Size(440, 28),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            grpCarnet.Controls.Add(new Label { Text = "Número:", Location = new Point(16, 72), AutoSize = true });
            txtNumero = CrearCampoSoloLectura(110, 68, 340);
            grpCarnet.Controls.Add(txtNumero);

            grpCarnet.Controls.Add(new Label { Text = "Emisión:", Location = new Point(16, 108), AutoSize = true });
            txtEmision = CrearCampoSoloLectura(110, 104, 160);
            grpCarnet.Controls.Add(txtEmision);

            grpCarnet.Controls.Add(new Label { Text = "Vencimiento:", Location = new Point(16, 144), AutoSize = true });
            txtVencimiento = CrearCampoSoloLectura(110, 140, 160);
            grpCarnet.Controls.Add(txtVencimiento);

            grpCarnet.Controls.Add(lblEstado);

            btnRenovar = new Button
            {
                Text = "Renovar (+1 año)",
                Location = new Point(12, 312),
                Size = new Size(150, 38),
                Enabled = false
            };
            UiTheme.AplicarBotonPrimario(btnRenovar);
            btnRenovar.Click += BtnRenovar_Click;

            btnEmitir = new Button
            {
                Text = "Emitir carnet",
                Location = new Point(172, 312),
                Size = new Size(130, 38),
                Enabled = false,
                Visible = false
            };
            UiTheme.AplicarBotonSecundario(btnEmitir);
            btnEmitir.Click += BtnEmitir_Click;

            lblMensaje = new Label
            {
                Location = new Point(12, 358),
                Size = new Size(476, 40),
                ForeColor = UiTheme.TextoSecundario,
                Font = new Font("Segoe UI", 9F)
            };

            Controls.AddRange([grpCarnet, btnRenovar, btnEmitir, lblMensaje, panelBusqueda]);

            AcceptButton = btnBuscar;
            txtDni.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    BuscarSocio();
                }
            };

            LimpiarCamposCarnet();
        }

        private static TextBox CrearCampoSoloLectura(int x, int y, int ancho)
        {
            var txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(ancho, 25),
                ReadOnly = true,
                BackColor = UiTheme.PrimarioClaro
            };
            UiTheme.AplicarCampo(txt);
            return txt;
        }

        private void BuscarSocio()
        {
            _socioSeleccionado = null;
            _carnetActual = null;
            LimpiarCamposCarnet();
            lblSocio.Text = string.Empty;
            lblMensaje.Text = string.Empty;

            var dni = txtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Ingrese un DNI.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _socioSeleccionado = _socioDao.ObtenerPorDni(dni);
                if (_socioSeleccionado is null)
                {
                    MessageBox.Show("Socio no encontrado.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                lblSocio.Text = $"{_socioSeleccionado.Nombre} {_socioSeleccionado.Apellido} — Nº socio {_socioSeleccionado.IdSocio}";

                _carnetActual = _carnetDao.ObtenerPorSocio(_socioSeleccionado.IdSocio);
                if (_carnetActual is null)
                {
                    lblEstado.Text = "Sin carnet registrado";
                    lblEstado.ForeColor = UiTheme.TextoSecundario;
                    btnRenovar.Enabled = false;
                    btnEmitir.Visible = true;
                    btnEmitir.Enabled = true;
                    lblMensaje.Text = "Puede emitir un carnet para este socio.";
                    return;
                }

                MostrarCarnet(_carnetActual);
                btnEmitir.Visible = false;
                btnRenovar.Enabled = true;
                lblMensaje.Text = _carnetActual.EstaVencido()
                    ? "El carnet está vencido. Use Renovar para extender la vigencia."
                    : "Carnet vigente.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarCarnet(Carnet carnet)
        {
            txtNumero.Text = carnet.Numero;
            txtEmision.Text = carnet.FechaEmision.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);
            txtVencimiento.Text = carnet.FechaVencimiento.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);

            lblEstado.Text = $"Estado: {carnet.EstadoTexto}";
            lblEstado.ForeColor = carnet.EstaVencido() ? UiTheme.Error : UiTheme.Exito;
        }

        private void BtnRenovar_Click(object? sender, EventArgs e)
        {
            if (_socioSeleccionado is null || _carnetActual is null)
            {
                return;
            }

            var nuevaVencimiento = DateTime.Today.AddYears(1);
            var confirmacion = MessageBox.Show(
                $"¿Renovar carnet {_carnetActual.Numero} hasta el {nuevaVencimiento:dd/MM/yyyy}?",
                "Confirmar renovación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            if (!_carnetDao.Renovar(_socioSeleccionado.IdSocio, nuevaVencimiento, out _))
            {
                MessageBox.Show("No se pudo renovar el carnet.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Carnet renovado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            BuscarSocio();
        }

        private void BtnEmitir_Click(object? sender, EventArgs e)
        {
            if (_socioSeleccionado is null)
            {
                return;
            }

            var carnet = new Carnet
            {
                SocioId = _socioSeleccionado.IdSocio,
                Numero = $"CARNET-{_socioSeleccionado.IdSocio:D4}",
                FechaEmision = DateTime.Today,
                FechaVencimiento = DateTime.Today.AddYears(1)
            };

            if (!_carnetDao.Crear(carnet, out var carnetId))
            {
                MessageBox.Show("No se pudo emitir el carnet.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"Carnet #{carnetId} emitido: {carnet.Numero}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            BuscarSocio();
        }

        private void Limpiar()
        {
            txtDni.Clear();
            _socioSeleccionado = null;
            _carnetActual = null;
            lblSocio.Text = string.Empty;
            lblMensaje.Text = string.Empty;
            LimpiarCamposCarnet();
            btnRenovar.Enabled = false;
            btnEmitir.Visible = false;
            txtDni.Focus();
        }

        private void LimpiarCamposCarnet()
        {
            txtNumero.Clear();
            txtEmision.Clear();
            txtVencimiento.Clear();
            lblEstado.Text = string.Empty;
        }
    }
}
