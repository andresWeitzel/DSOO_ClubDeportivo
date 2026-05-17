using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using TP_ClubDeportivo.DAO;
using TP_ClubDeportivo.Models;

namespace TP_ClubDeportivo.Forms
{
    public class FormVisitantes : Form
    {
        private readonly SplitContainer split;
        private readonly DataGridView dgvVisitantes;
        private readonly GroupBox grpFormulario;
        private readonly Panel panelModo;
        private readonly Label lblModo;
        private readonly TextBox txtDni, txtNombre, txtApellido, txtTelefono, txtActividad;
        private readonly NumericUpDown numMonto;
        private readonly ComboBox cboMedioPago;
        private readonly Button btnGuardar;
        private readonly Button btnNuevo;
        private readonly Button btnEliminar;
        private readonly Button btnRegistrarPago;
        private readonly Label lblMensaje;

        private readonly VisitanteDAO _visitanteDao = new();
        private readonly PagoDAO _pagoDao = new();

        private int? _visitanteSeleccionadoId;
        private bool _tienePagoRegistrado;

        public FormVisitantes()
        {
            Text = "Ingreso de Visitantes";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1050, 620);
            MinimumSize = new Size(980, 560);
            Font = UiTheme.FuenteNormal;
            BackColor = UiTheme.Fondo;

            split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                BackColor = UiTheme.Fondo
            };

            var panelGrilla = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };

            var btnRefrescar = new Button
            {
                Text = "Refrescar lista",
                Dock = DockStyle.Top,
                Height = 36
            };
            UiTheme.AplicarBotonSecundario(btnRefrescar);
            btnRefrescar.Click += (_, _) => CargarVisitantes();

            dgvVisitantes = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false
            };
            dgvVisitantes.SelectionChanged += DgvVisitantes_SelectionChanged;

            panelGrilla.Controls.Add(dgvVisitantes);
            panelGrilla.Controls.Add(btnRefrescar);
            split.Panel1.Controls.Add(panelGrilla);

            grpFormulario = new GroupBox
            {
                Text = "Datos del visitante",
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 16, 12, 12),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            panelModo = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = UiTheme.PrimarioClaro,
                Padding = new Padding(10, 8, 10, 8)
            };

            lblModo = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UiTheme.PrimarioOscuro,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Text = "Alta: complete los datos y registre el ingreso con su pago.",
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelModo.Controls.Add(lblModo);

            var panelCampos = new Panel
            {
                Dock = DockStyle.Top,
                Height = 248,
                Padding = new Padding(4, 8, 4, 0)
            };

            const int anchoEtiqueta = 115;
            const int anchoCampo = 230;
            int y = 0;

            txtDni = AgregarCampo(panelCampos, "DNI:", anchoEtiqueta, anchoCampo, ref y);
            txtNombre = AgregarCampo(panelCampos, "Nombre:", anchoEtiqueta, anchoCampo, ref y);
            txtApellido = AgregarCampo(panelCampos, "Apellido:", anchoEtiqueta, anchoCampo, ref y);
            txtTelefono = AgregarCampo(panelCampos, "Teléfono:", anchoEtiqueta, anchoCampo, ref y);
            txtActividad = AgregarCampo(panelCampos, "Actividad:", anchoEtiqueta, anchoCampo, ref y);

            panelCampos.Controls.Add(new Label
            {
                Text = "Pago diario ($):",
                Location = new Point(4, y + 4),
                Size = new Size(anchoEtiqueta, 22),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = UiTheme.FuenteNormal
            });
            numMonto = new NumericUpDown
            {
                Location = new Point(anchoEtiqueta + 8, y),
                Size = new Size(anchoCampo, 25),
                DecimalPlaces = 2,
                Minimum = 0.01m,
                Maximum = 999999m,
                Value = 50m,
                ThousandsSeparator = true
            };
            panelCampos.Controls.Add(numMonto);
            y += 36;

            panelCampos.Controls.Add(new Label
            {
                Text = "Medio de pago:",
                Location = new Point(4, y + 4),
                Size = new Size(anchoEtiqueta, 22),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = UiTheme.FuenteNormal
            });
            cboMedioPago = new ComboBox
            {
                Location = new Point(anchoEtiqueta + 8, y),
                Size = new Size(anchoCampo, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = UiTheme.FuenteNormal
            };
            cboMedioPago.Items.AddRange(["Efectivo", "Tarjeta Débito", "Tarjeta Crédito", "Transferencia"]);
            cboMedioPago.SelectedIndex = 0;
            panelCampos.Controls.Add(cboMedioPago);

            var panelBotones = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                Padding = new Padding(4, 12, 4, 0)
            };

            btnGuardar = new Button
            {
                Text = "Registrar ingreso",
                Location = new Point(4, 0),
                Size = new Size(165, 38)
            };
            UiTheme.AplicarBotonPrimario(btnGuardar);
            btnGuardar.Click += BtnGuardar_Click;

            btnNuevo = new Button
            {
                Text = "Nuevo",
                Location = new Point(178, 0),
                Size = new Size(90, 38)
            };
            UiTheme.AplicarBotonSecundario(btnNuevo);
            btnNuevo.Click += (_, _) => ModoNuevo();

            btnEliminar = new Button
            {
                Text = "Eliminar",
                Location = new Point(276, 0),
                Size = new Size(90, 38),
                Enabled = false
            };
            UiTheme.AplicarBotonSecundario(btnEliminar);
            btnEliminar.Click += BtnEliminar_Click;

            btnRegistrarPago = new Button
            {
                Text = "Registrar pago pendiente",
                Location = new Point(4, 46),
                Size = new Size(362, 32),
                Enabled = false,
                Visible = false
            };
            UiTheme.AplicarBotonSecundario(btnRegistrarPago);
            btnRegistrarPago.Click += BtnRegistrarPago_Click;

            panelBotones.Controls.AddRange([btnGuardar, btnNuevo, btnEliminar, btnRegistrarPago]);

            lblMensaje = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UiTheme.TextoSecundario,
                Padding = new Padding(6, 8, 6, 0),
                Font = new Font("Segoe UI", 9F)
            };

            grpFormulario.Controls.Add(lblMensaje);
            grpFormulario.Controls.Add(panelBotones);
            grpFormulario.Controls.Add(panelCampos);
            grpFormulario.Controls.Add(panelModo);

            split.Panel2.Controls.Add(grpFormulario);
            Controls.Add(split);

            Load += (_, _) =>
            {
                UiTheme.ConfigurarSplitVertical(split, 0.58);
                ModoNuevo();
                CargarVisitantes();
            };
        }

        private static TextBox AgregarCampo(Panel panel, string etiqueta, int anchoEtiqueta, int anchoCampo, ref int y)
        {
            panel.Controls.Add(new Label
            {
                Text = etiqueta,
                Location = new Point(4, y + 4),
                Size = new Size(anchoEtiqueta, 22),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = UiTheme.FuenteNormal
            });
            var txt = new TextBox
            {
                Location = new Point(anchoEtiqueta + 8, y),
                Size = new Size(anchoCampo, 25)
            };
            UiTheme.AplicarCampo(txt);
            panel.Controls.Add(txt);
            y += 36;
            return txt;
        }

        private void DgvVisitantes_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvVisitantes.CurrentRow?.DataBoundItem is not VisitanteListado fila)
            {
                return;
            }

            ModoEdicion(fila);
        }

        private void ModoEdicion(VisitanteListado fila)
        {
            _visitanteSeleccionadoId = fila.IdVisitante;
            _tienePagoRegistrado = fila.PagoRegistrado == "Sí";

            grpFormulario.Text = $"Editar visitante Nº {fila.IdVisitante}";
            lblModo.Text = _tienePagoRegistrado
                ? "Edición: puede modificar datos, monto y medio de pago del último cobro."
                : "Edición: sin pago registrado. Guarde cambios o use «Registrar pago pendiente».";
            btnGuardar.Text = "Guardar cambios";
            btnEliminar.Enabled = true;

            txtDni.Text = fila.Dni;
            txtNombre.Text = fila.Nombre;
            txtApellido.Text = fila.Apellido;
            txtTelefono.Text = fila.Telefono;
            txtActividad.Text = fila.Actividad;
            EstablecerMonto(fila.Monto);

            cboMedioPago.Enabled = true;
            SeleccionarMedioPago(fila.MedioPago);

            btnRegistrarPago.Visible = !_tienePagoRegistrado;
            btnRegistrarPago.Enabled = !_tienePagoRegistrado;

            lblMensaje.Text = _tienePagoRegistrado
                ? "Al guardar se actualizan el visitante y su último pago."
                : "Registre el pago cuando el visitante abone la entrada.";
        }

        private void ModoNuevo()
        {
            _visitanteSeleccionadoId = null;
            _tienePagoRegistrado = false;
            dgvVisitantes.ClearSelection();

            grpFormulario.Text = "Nuevo visitante (CU-02)";
            lblModo.Text = "Alta: complete los datos y registre el ingreso con su pago diario.";
            btnGuardar.Text = "Registrar ingreso";
            btnEliminar.Enabled = false;
            btnRegistrarPago.Visible = false;
            btnRegistrarPago.Enabled = false;
            cboMedioPago.Enabled = true;

            LimpiarCampos();
            lblMensaje.Text = string.Empty;
        }

        private void CargarVisitantes()
        {
            var idPrevio = _visitanteSeleccionadoId;

            try
            {
                dgvVisitantes.DataSource = _visitanteDao.ObtenerListado().ToList();
                ConfigurarColumnasGrilla();

                if (idPrevio.HasValue)
                {
                    foreach (DataGridViewRow row in dgvVisitantes.Rows)
                    {
                        if (row.DataBoundItem is VisitanteListado v && v.IdVisitante == idPrevio.Value)
                        {
                            row.Selected = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarColumnasGrilla()
        {
            if (dgvVisitantes.Columns.Count == 0)
            {
                return;
            }

            var columnas = new Dictionary<string, string>
            {
                ["IdVisitante"] = "Nº",
                ["Dni"] = "DNI",
                ["Nombre"] = "Nombre",
                ["Apellido"] = "Apellido",
                ["Telefono"] = "Teléfono",
                ["Actividad"] = "Actividad",
                ["FechaIngreso"] = "Fecha ingreso",
                ["Monto"] = "Monto ($)",
                ["MedioPago"] = "Medio de pago",
                ["PagoRegistrado"] = "Pago OK"
            };

            foreach (var par in columnas)
            {
                if (dgvVisitantes.Columns.Contains(par.Key))
                {
                    dgvVisitantes.Columns[par.Key]!.HeaderText = par.Value;
                }
            }

            if (dgvVisitantes.Columns.Contains("FechaIngreso"))
            {
                dgvVisitantes.Columns["FechaIngreso"]!.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }

            if (dgvVisitantes.Columns.Contains("Monto"))
            {
                dgvVisitantes.Columns["Monto"]!.DefaultCellStyle.Format = "N2";
                dgvVisitantes.Columns["Monto"]!.DefaultCellStyle.FormatProvider = CultureInfo.CurrentCulture;
            }
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (!ValidarCampos(out var monto))
            {
                return;
            }

            try
            {
                if (_visitanteSeleccionadoId.HasValue)
                {
                    ActualizarVisitante(monto);
                }
                else
                {
                    CrearVisitante(monto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CrearVisitante(decimal monto)
        {
            var visitante = ConstruirVisitante(monto);

            if (!_visitanteDao.Crear(visitante, out var visitanteId))
            {
                lblMensaje.Text = "No se pudo registrar el visitante.";
                return;
            }

            var concepto = $"Entrada diaria — {visitante.Actividad}";
            if (!_pagoDao.RegistrarPagoVisitante(visitanteId, monto, cboMedioPago.Text, concepto, out var pagoId))
            {
                lblMensaje.Text = $"Visitante #{visitanteId} creado. No se registró el pago.";
            }
            else
            {
                lblMensaje.Text = $"Visitante #{visitanteId} — Pago #{pagoId} por {FormatearMonto(monto)}.";
                MessageBox.Show("Ingreso y pago registrados.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            ModoNuevo();
            CargarVisitantes();
        }

        private void ActualizarVisitante(decimal monto)
        {
            var visitante = ConstruirVisitante(monto);
            visitante.IdVisitante = _visitanteSeleccionadoId!.Value;

            if (!_visitanteDao.Actualizar(visitante))
            {
                lblMensaje.Text = "No se pudieron guardar los cambios del visitante.";
                return;
            }

            if (_tienePagoRegistrado)
            {
                if (!_pagoDao.ActualizarUltimoPagoVisitante(visitante.IdVisitante, monto, cboMedioPago.Text))
                {
                    lblMensaje.Text = "Visitante actualizado, pero no se pudo actualizar el pago.";
                    CargarVisitantes();
                    return;
                }
            }

            lblMensaje.Text = $"Visitante #{visitante.IdVisitante} actualizado correctamente.";
            MessageBox.Show("Cambios guardados.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CargarVisitantes();
        }

        private void BtnRegistrarPago_Click(object? sender, EventArgs e)
        {
            if (!_visitanteSeleccionadoId.HasValue || _tienePagoRegistrado)
            {
                return;
            }

            if (!ValidarCampos(out var monto))
            {
                return;
            }

            var visitante = ConstruirVisitante(monto);
            visitante.IdVisitante = _visitanteSeleccionadoId.Value;

            if (!_visitanteDao.Actualizar(visitante))
            {
                MessageBox.Show("No se pudo actualizar el monto del visitante.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var concepto = $"Entrada diaria — {txtActividad.Text.Trim()}";
            if (!_pagoDao.RegistrarPagoVisitante(_visitanteSeleccionadoId.Value, monto, cboMedioPago.Text, concepto, out var pagoId))
            {
                MessageBox.Show("No se pudo registrar el pago.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"Pago #{pagoId} registrado por {FormatearMonto(monto)}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CargarVisitantes();
        }

        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            if (!_visitanteSeleccionadoId.HasValue)
            {
                return;
            }

            var textoConfirmacion = _tienePagoRegistrado
                ? $"¿Eliminar al visitante #{_visitanteSeleccionadoId.Value}?\n\nTambién se eliminarán sus pagos asociados."
                : $"¿Eliminar al visitante #{_visitanteSeleccionadoId.Value}?";

            if (MessageBox.Show(textoConfirmacion, "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            if (!_visitanteDao.Eliminar(_visitanteSeleccionadoId.Value, out var mensaje))
            {
                MessageBox.Show(
                    string.IsNullOrWhiteSpace(mensaje) ? "No se pudo eliminar el visitante." : mensaje,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(mensaje, "Eliminado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ModoNuevo();
            CargarVisitantes();
        }

        private Visitante ConstruirVisitante(decimal monto)
        {
            return new Visitante
            {
                DNI = txtDni.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Actividad = txtActividad.Text.Trim(),
                PagoDiarioMonto = monto
            };
        }

        private bool ValidarCampos(out decimal monto)
        {
            monto = numMonto.Value;
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtActividad.Text))
            {
                MessageBox.Show("Complete al menos nombre y actividad.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (monto <= 0)
            {
                MessageBox.Show("Ingrese un monto mayor a cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void EstablecerMonto(decimal monto)
        {
            var valor = Math.Clamp(monto, numMonto.Minimum, numMonto.Maximum);
            numMonto.Value = valor;
        }

        private void SeleccionarMedioPago(string medio)
        {
            if (string.IsNullOrWhiteSpace(medio))
            {
                cboMedioPago.SelectedIndex = 0;
                return;
            }

            var idx = cboMedioPago.Items.IndexOf(medio);
            cboMedioPago.SelectedIndex = idx >= 0 ? idx : 0;
        }

        private static string FormatearMonto(decimal monto) =>
            monto.ToString("C2", CultureInfo.CurrentCulture);

        private void LimpiarCampos()
        {
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtTelefono.Clear();
            txtActividad.Clear();
            numMonto.Value = 50m;
            cboMedioPago.SelectedIndex = 0;
        }
    }
}
