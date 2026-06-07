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
            Size = new Size(560, 480);
            MinimumSize = new Size(520, 440);
            Font = UiTheme.FuenteNormal;
            BackColor = UiTheme.Fondo;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;

            txtDni = new TextBox { PlaceholderText = "Ej: 12345678" };
            UiTheme.AplicarCampo(txtDni);

            btnBuscar = new Button { Text = "Buscar" };
            UiTheme.AjustarBotonToolbar(btnBuscar, primario: true);
            btnBuscar.Click += (_, _) => BuscarSocio();

            btnLimpiar = new Button { Text = "Limpiar" };
            UiTheme.AjustarBotonToolbar(btnLimpiar);
            btnLimpiar.Click += (_, _) => Limpiar();

            lblSocio = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = UiTheme.Primario
            };

            var panelBusqueda = UiTheme.CrearPanelBusqueda(
                "DNI del socio:",
                txtDni,
                [btnBuscar, btnLimpiar],
                lblSocio);

            var grpCarnet = new GroupBox
            {
                Text = "Datos del carnet",
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 20, 16, 12),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Margin = new Padding(12, 8, 12, 8)
            };

            lblEstado = new Label
            {
                Dock = DockStyle.Top,
                Height = 32,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var layoutCarnet = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(0, 8, 0, 0)
            };
            layoutCarnet.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layoutCarnet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (var i = 0; i < 3; i++)
            {
                layoutCarnet.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            }

            layoutCarnet.Controls.Add(new Label { Text = "Número:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 10, 8, 0) }, 0, 0);
            txtNumero = CrearCampoSoloLectura();
            layoutCarnet.Controls.Add(txtNumero, 1, 0);

            layoutCarnet.Controls.Add(new Label { Text = "Emisión:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 10, 8, 0) }, 0, 1);
            txtEmision = CrearCampoSoloLectura();
            layoutCarnet.Controls.Add(txtEmision, 1, 1);

            layoutCarnet.Controls.Add(new Label { Text = "Vencimiento:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 10, 8, 0) }, 0, 2);
            txtVencimiento = CrearCampoSoloLectura();
            layoutCarnet.Controls.Add(txtVencimiento, 1, 2);

            grpCarnet.Controls.Add(layoutCarnet);
            grpCarnet.Controls.Add(lblEstado);

            btnRenovar = new Button { Text = "Renovar (+1 año)", Enabled = false };
            UiTheme.AplicarBotonPrimario(btnRenovar);
            btnRenovar.Click += BtnRenovar_Click;

            btnEmitir = new Button { Text = "Emitir carnet", Enabled = false, Visible = false };
            UiTheme.AplicarBotonSecundario(btnEmitir);
            btnEmitir.Click += BtnEmitir_Click;

            var panelBotones = UiTheme.CrearBarraBotones(btnRenovar, btnEmitir);
            panelBotones.Dock = DockStyle.Bottom;
            panelBotones.Padding = new Padding(12, 8, 12, 12);

            lblMensaje = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                ForeColor = UiTheme.TextoSecundario,
                Font = new Font("Segoe UI", 9F),
                Padding = new Padding(12, 0, 12, 8)
            };

            var panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 0, 12, 0)
            };
            panelContenido.Controls.Add(grpCarnet);

            Controls.Add(panelContenido);
            Controls.Add(lblMensaje);
            Controls.Add(panelBotones);
            Controls.Add(panelBusqueda);

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

        private static TextBox CrearCampoSoloLectura()
        {
            var txt = new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = UiTheme.PrimarioClaro,
                Margin = new Padding(0, 4, 0, 0)
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
