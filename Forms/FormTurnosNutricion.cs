using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TP_ClubDeportivo.DAO;
using TP_ClubDeportivo.Models;

namespace TP_ClubDeportivo.Forms
{
    public class FormTurnosNutricion : Form
    {
        private static readonly string[] CargasPermitidas =
        [
            "Sin restricciones",
            "Actividad moderada",
            "Entrenamiento controlado",
            "Cardio y máquinas guiadas",
            "Yoga y pilates liviano",
            "Evitar >50kg en espalda",
            "Pendiente de evaluación",
            "Sin definir"
        ];

        private readonly SocioDAO _socioDao = new();
        private readonly NutricionistaDAO _nutricionistaDao = new();
        private readonly TurnoNutricionDAO _turnoDao = new();
        private readonly FichaMedicaDAO _fichaDao = new();

        private Socio? _socioSeleccionado;
        private (int Id, int SocioId, string SocioNombre, decimal Peso, decimal Altura, string Alergias, string Medicacion, string Observaciones, string CargaPermitida)? _fichaSeleccionada;
        private TurnoSocioViewModel? _turnoConsultaSeleccionado;
        private List<TurnoDisponibleViewModel> _turnosDisponibles = new();
        private List<TurnoSocioViewModel> _turnosSocio = new();

        private readonly TextBox txtDni;
        private readonly Button btnBuscarSocio;
        private readonly Label lblSocio;
        private readonly Label lblEstadoCuota;
        private readonly Label lblFichaMedica;
        private readonly ComboBox cbNutricionistas;
        private readonly DateTimePicker dtpFecha;
        private readonly ComboBox cbHorasDisponibles;
        private readonly Button btnBuscarSiguienteSemana;
        private readonly Button btnAsignarTurno;
        private readonly Label lblFechaDisponibles;
        private readonly Label lblMensaje;
        private readonly DataGridView dgvTurnosSocio;
        private readonly NumericUpDown numPeso;
        private readonly NumericUpDown numAltura;
        private readonly TextBox txtAlergias;
        private readonly TextBox txtMedicacion;
        private readonly TextBox txtObservacionesFicha;
        private readonly ComboBox cbCargaPermitida;
        private readonly Button btnGuardarConsulta;
        private readonly Label lblConsultaMensaje;
        private bool _cargandoHorarios;

        public FormTurnosNutricion()
        {
            Text = "Gestionar turno de nutrición (CU-07)";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1280, 780);
            MinimumSize = new Size(1100, 700);
            Font = UiTheme.FuenteNormal;
            BackColor = UiTheme.Fondo;

            txtDni = new TextBox { PlaceholderText = "Ej: 12345678" };
            UiTheme.AplicarCampo(txtDni);
            txtDni.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    BuscarSocio();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };

            btnBuscarSocio = new Button { Text = "Buscar socio" };
            UiTheme.AjustarBotonToolbar(btnBuscarSocio, primario: true);
            btnBuscarSocio.Click += (_, _) => BuscarSocio();

            lblSocio = new Label
            {
                Text = "Socio: -",
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = UiTheme.Primario
            };

            lblEstadoCuota = new Label
            {
                Text = "Estado cuota: -",
                AutoSize = true,
                Font = UiTheme.FuenteNormal,
                ForeColor = UiTheme.TextoSecundario
            };

            lblFichaMedica = new Label
            {
                Text = "Ficha médica: -",
                AutoSize = true,
                MaximumSize = new Size(900, 0),
                Font = UiTheme.FuenteNormal,
                ForeColor = UiTheme.TextoSecundario
            };

            var panelBusqueda = UiTheme.CrearPanelBusqueda(
                "DNI del socio:",
                txtDni,
                [btnBuscarSocio],
                lblSocio,
                lblEstadoCuota,
                lblFichaMedica);
            panelBusqueda.Dock = DockStyle.Top;

            dgvTurnosSocio = CrearGrilla();
            dgvTurnosSocio.Dock = DockStyle.Fill;
            dgvTurnosSocio.SelectionChanged += (_, _) => SeleccionarTurnoParaConsulta();

            var panelConsulta = new GroupBox
            {
                Text = "Consulta — actualizar ficha",
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 12, 12, 8),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = UiTheme.Tarjeta
            };

            var panelConsultaCampos = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(0, 0, 4, 0)
            };

            var panelConsultaAcciones = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 92,
                Padding = new Padding(0, 8, 0, 0)
            };

            panelConsultaCampos.Controls.Add(new Label
            {
                Text = "Peso (kg):",
                Location = new Point(12, 28),
                AutoSize = true,
                Font = UiTheme.FuenteNormal
            });

            numPeso = new NumericUpDown
            {
                Location = new Point(12, 48),
                Width = 100,
                DecimalPlaces = 2,
                Maximum = 300,
                Minimum = 0,
                Increment = 0.5M
            };

            panelConsultaCampos.Controls.Add(new Label
            {
                Text = "Altura (m):",
                Location = new Point(130, 28),
                AutoSize = true,
                Font = UiTheme.FuenteNormal
            });

            numAltura = new NumericUpDown
            {
                Location = new Point(130, 48),
                Width = 100,
                DecimalPlaces = 2,
                Maximum = 2.5M,
                Minimum = 0,
                Increment = 0.01M
            };

            panelConsultaCampos.Controls.Add(new Label
            {
                Text = "Alergias:",
                Location = new Point(12, 82),
                AutoSize = true,
                Font = UiTheme.FuenteNormal
            });

            txtAlergias = new TextBox
            {
                Location = new Point(12, 102),
                Width = 218,
                PlaceholderText = "Ej: Penicilina"
            };
            UiTheme.AplicarCampo(txtAlergias);

            panelConsultaCampos.Controls.Add(new Label
            {
                Text = "Medicación:",
                Location = new Point(12, 132),
                AutoSize = true,
                Font = UiTheme.FuenteNormal
            });

            txtMedicacion = new TextBox
            {
                Location = new Point(12, 152),
                Width = 218,
                PlaceholderText = "Ej: Ninguna"
            };
            UiTheme.AplicarCampo(txtMedicacion);

            panelConsultaCampos.Controls.Add(new Label
            {
                Text = "Observaciones:",
                Location = new Point(12, 182),
                AutoSize = true,
                Font = UiTheme.FuenteNormal
            });

            txtObservacionesFicha = new TextBox
            {
                Location = new Point(12, 202),
                Width = 218,
                Height = 88,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                PlaceholderText = "Notas de la consulta"
            };
            UiTheme.AplicarCampo(txtObservacionesFicha);

            panelConsultaCampos.Controls.Add(new Label
            {
                Text = "Carga actividad permitida:",
                Location = new Point(12, 278),
                AutoSize = true,
                Font = UiTheme.FuenteNormal
            });

            cbCargaPermitida = new ComboBox
            {
                Location = new Point(12, 286),
                Width = 218,
                DropDownStyle = ComboBoxStyle.DropDown
            };
            cbCargaPermitida.Items.AddRange(CargasPermitidas);

            btnGuardarConsulta = new Button
            {
                Text = "Guardar ficha",
                Dock = DockStyle.Top,
                Height = 38
            };
            UiTheme.AplicarBotonPrimario(btnGuardarConsulta);
            btnGuardarConsulta.Click += (_, _) => GuardarConsulta();
            btnGuardarConsulta.Enabled = false;

            lblConsultaMensaje = new Label
            {
                Text = "Busque un socio para actualizar la ficha médica.",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F),
                ForeColor = UiTheme.TextoSecundario
            };

            panelConsultaCampos.Controls.AddRange(
            [
                numPeso, numAltura, txtAlergias, txtMedicacion, txtObservacionesFicha, cbCargaPermitida
            ]);

            panelConsultaAcciones.Controls.Add(lblConsultaMensaje);
            panelConsultaAcciones.Controls.Add(btnGuardarConsulta);
            panelConsulta.Controls.Add(panelConsultaCampos);
            panelConsulta.Controls.Add(panelConsultaAcciones);

            void AjustarAnchoCamposConsulta()
            {
                const int margen = 12;
                const int separacion = 10;
                var anchoTotal = Math.Max(280, panelConsultaCampos.ClientSize.Width - margen * 2);
                var anchoMitad = (anchoTotal - separacion) / 2;

                numPeso.Width = anchoMitad;
                numPeso.Location = new Point(margen, 48);

                numAltura.Width = anchoMitad;
                numAltura.Location = new Point(margen + anchoMitad + separacion, 48);

                txtAlergias.Width = anchoTotal;
                txtAlergias.Location = new Point(margen, 102);

                txtMedicacion.Width = anchoTotal;
                txtMedicacion.Location = new Point(margen, 152);

                txtObservacionesFicha.Width = anchoTotal;
                txtObservacionesFicha.Location = new Point(margen, 202);

                cbCargaPermitida.Width = anchoTotal;
                cbCargaPermitida.Location = new Point(margen, 298);
            }

            panelConsultaCampos.Resize += (_, _) => AjustarAnchoCamposConsulta();

            var splitCentral = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                BackColor = UiTheme.Fondo
            };
            splitCentral.Panel1.Controls.Add(dgvTurnosSocio);
            splitCentral.Panel2.Controls.Add(panelConsulta);

            Load += (_, _) =>
            {
                if (!Permisos.ValidarAccesoAlAbrir(this, Permisos.Modulo.TurnosNutricion))
                {
                    return;
                }

                UiTheme.ConfigurarSplitVertical(splitCentral, ratioPanel1: 0.38, panel1Min: 240, panel2Min: 320);
                AjustarAnchoCamposConsulta();
                CargarNutricionistas();
            };

            var panelGrilla = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                BackColor = UiTheme.Fondo
            };
            panelGrilla.Controls.Add(splitCentral);

            var panelAsignacion = new GroupBox
            {
                Text = "Asignar turno nutrición",
                Dock = DockStyle.Bottom,
                Height = 260,
                Padding = new Padding(16, 20, 16, 12),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = UiTheme.Tarjeta
            };

            var layoutAsignacion = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 5,
                Padding = new Padding(0, 4, 0, 0)
            };
            layoutAsignacion.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            layoutAsignacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutAsignacion.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            for (var i = 0; i < 5; i++)
            {
                layoutAsignacion.RowStyles.Add(new RowStyle(SizeType.Absolute, i == 3 ? 28F : 44F));
            }

            layoutAsignacion.Controls.Add(new Label
            {
                Text = "Nutricionista:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 10, 8, 0),
                Font = UiTheme.FuenteNormal
            }, 0, 0);

            cbNutricionistas = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 6, 0, 0)
            };
            cbNutricionistas.SelectedIndexChanged += OnNutricionistaCambiado;
            layoutAsignacion.SetColumnSpan(cbNutricionistas, 2);
            layoutAsignacion.Controls.Add(cbNutricionistas, 1, 0);

            layoutAsignacion.Controls.Add(new Label
            {
                Text = "Fecha:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 10, 8, 0),
                Font = UiTheme.FuenteNormal
            }, 0, 1);

            dtpFecha = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short,
                MinDate = DateTime.Today,
                Margin = new Padding(0, 6, 0, 0)
            };
            dtpFecha.ValueChanged += OnFechaCambiada;
            layoutAsignacion.SetColumnSpan(dtpFecha, 2);
            layoutAsignacion.Controls.Add(dtpFecha, 1, 1);

            layoutAsignacion.Controls.Add(new Label
            {
                Text = "Hora disponible:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 10, 8, 0),
                Font = UiTheme.FuenteNormal
            }, 0, 2);

            cbHorasDisponibles = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 6, 8, 0),
                MinimumSize = new Size(140, 0)
            };
            cbHorasDisponibles.SelectedIndexChanged += (_, _) => ActualizarEstadoFormulario();
            layoutAsignacion.Controls.Add(cbHorasDisponibles, 1, 2);

            btnBuscarSiguienteSemana = new Button { Text = "Buscar próxima fecha" };
            UiTheme.AjustarBotonToolbar(btnBuscarSiguienteSemana);
            btnBuscarSiguienteSemana.Margin = new Padding(0, 6, 0, 0);
            btnBuscarSiguienteSemana.Click += (_, _) => BuscarProximaFechaDisponible();
            layoutAsignacion.Controls.Add(btnBuscarSiguienteSemana, 2, 2);

            lblFechaDisponibles = new Label
            {
                Text = "Turnos disponibles para: -",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = UiTheme.FuenteNormal,
                ForeColor = UiTheme.TextoSecundario,
                Margin = new Padding(0, 4, 0, 0)
            };
            layoutAsignacion.SetColumnSpan(lblFechaDisponibles, 3);
            layoutAsignacion.Controls.Add(lblFechaDisponibles, 0, 3);

            btnAsignarTurno = new Button { Text = "Asignar turno" };
            UiTheme.AjustarBotonToolbar(btnAsignarTurno, primario: true);
            btnAsignarTurno.Margin = new Padding(0, 6, 0, 0);
            btnAsignarTurno.Click += (_, _) => AsignarTurno();
            layoutAsignacion.Controls.Add(btnAsignarTurno, 0, 4);
            layoutAsignacion.SetColumnSpan(btnAsignarTurno, 1);

            lblMensaje = new Label
            {
                Text = "Busque un socio y seleccione un turno disponible.",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = UiTheme.TextoSecundario,
                Margin = new Padding(12, 12, 0, 0)
            };
            layoutAsignacion.SetColumnSpan(lblMensaje, 2);
            layoutAsignacion.Controls.Add(lblMensaje, 1, 4);

            panelAsignacion.Controls.Add(layoutAsignacion);

            var panelSuperior = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0)
            };

            panelSuperior.Controls.Add(panelGrilla);
            panelSuperior.Controls.Add(panelAsignacion);

            Controls.Add(panelSuperior);
            Controls.Add(panelBusqueda);
        }

        private void OnFechaCambiada(object? sender, EventArgs e)
        {
            if (_cargandoHorarios)
            {
                return;
            }

            CargarTurnosDisponibles();
            ActualizarEstadoFormulario();
        }

        private void CargarNutricionistas()
        {
            try
            {
                cbNutricionistas.SelectedIndexChanged -= OnNutricionistaCambiado;
                var nutricionistas = _nutricionistaDao.ObtenerTodos().ToList();
                cbNutricionistas.Items.Clear();

                foreach (var nutricionista in nutricionistas)
                {
                    cbNutricionistas.Items.Add(new NutricionistaItem(nutricionista));
                }

                if (cbNutricionistas.Items.Count > 0)
                {
                    cbNutricionistas.SelectedIndex = 0;
                }

                CargarTurnosDisponibles();
            }
            catch
            {
                MessageBox.Show("No se pudieron cargar los nutricionistas. Verifique la conexión a la base de datos.", "Turnos nutrición", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cbNutricionistas.SelectedIndexChanged += OnNutricionistaCambiado;
                ActualizarEstadoFormulario();
            }
        }

        private void OnNutricionistaCambiado(object? sender, EventArgs e)
        {
            CargarTurnosDisponibles();
            ActualizarEstadoFormulario();
        }

        private void BuscarSocio()
        {
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
                    lblSocio.Text = "Socio: no encontrado.";
                    lblEstadoCuota.Text = "Estado cuota: -";
                    lblFichaMedica.Text = "Ficha médica: -";
                    dgvTurnosSocio.DataSource = null;
                    LimpiarConsulta();
                    lblMensaje.Text = "No se encontró un socio con ese DNI.";
                    lblMensaje.ForeColor = Color.DarkRed;
                    btnAsignarTurno.Enabled = false;
                    return;
                }

                lblSocio.Text = $"Socio: {_socioSeleccionado.Nombre} {_socioSeleccionado.Apellido} (#{_socioSeleccionado.IdSocio})";
                lblEstadoCuota.Text = $"Estado cuota: {_socioSeleccionado.EstadoCuota}";
                lblEstadoCuota.ForeColor = _socioSeleccionado.EstadoCuota.Equals("AL_DIA", StringComparison.OrdinalIgnoreCase)
                    ? Color.DarkGreen
                    : Color.OrangeRed;

                CargarFichaMedica();
                CargarTurnosSocio();
                CargarTurnosDisponibles();
                ActualizarEstadoFormulario();
                ActualizarEstadoConsulta();
            }
            catch
            {
                MessageBox.Show("Ocurrió un error al buscar el socio. Verifique la conexión a la base de datos.", "Turnos nutrición", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarTurnosSocio()
        {
            if (_socioSeleccionado is null)
            {
                dgvTurnosSocio.DataSource = null;
                return;
            }

            try
            {
                _turnosSocio = _turnoDao.ObtenerPorSocio(_socioSeleccionado.IdSocio)
                    .Select(t => new TurnoSocioViewModel
                    {
                        Id = t.Id,
                        Nutricionista = t.NutricionistaNombre,
                        Fecha = t.Fecha,
                        Hora = t.Hora,
                        Estado = t.Estado
                    })
                    .ToList();

                dgvTurnosSocio.DataSource = _turnosSocio;
                ConfigurarColumnasTurnosSocio();
            }
            catch
            {
                MessageBox.Show("No se pudieron cargar los turnos del socio. Verifique la conexión.", "Turnos nutrición", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuscarProximaFechaDisponible()
        {
            if (cbNutricionistas.SelectedItem is not NutricionistaItem nutricionistaItem)
            {
                MessageBox.Show("Seleccione un nutricionista.", "Turnos nutrición", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var fechaActual = dtpFecha.Value.Date;
                if (fechaActual < DateTime.Today)
                {
                    AsegurarFechaNoPasada();
                    fechaActual = dtpFecha.Value.Date;
                }

                var horariosHoy = _turnoDao.ObtenerHorariosDisponibles(fechaActual, nutricionistaItem.Id).ToList();
                if (horariosHoy.Count > 0)
                {
                    PoblarHorariosDisponibles(fechaActual, horariosHoy);
                    lblMensaje.Text = $"Hay {horariosHoy.Count} horario(s) el {fechaActual:dd/MM/yyyy}. Seleccione uno y busque un socio.";
                    lblMensaje.ForeColor = UiTheme.Primario;
                    ActualizarEstadoFormulario();
                    return;
                }

                var inicioBusqueda = fechaActual.AddDays(1);
                if (inicioBusqueda < DateTime.Today)
                {
                    inicioBusqueda = DateTime.Today;
                }

                var (fecha, horarios) = _turnoDao.BuscarProximoCupo(inicioBusqueda, nutricionistaItem.Id);
                if (fecha is null || horarios.Count == 0)
                {
                    cbHorasDisponibles.Items.Clear();
                    cbHorasDisponibles.Text = string.Empty;
                    lblFechaDisponibles.Text = "Sin horarios libres en los próximos 90 días.";
                    lblMensaje.Text = "E1: No hay horarios libres en los próximos 90 días para este nutricionista.";
                    lblMensaje.ForeColor = Color.DarkRed;
                    ActualizarEstadoFormulario();
                    return;
                }

                dtpFecha.ValueChanged -= OnFechaCambiada;
                dtpFecha.Value = fecha.Value;
                dtpFecha.ValueChanged += OnFechaCambiada;

                PoblarHorariosDisponibles(fecha.Value, horarios);
                lblMensaje.Text = $"Próximo cupo: {fecha.Value:dd/MM/yyyy} a las {FormatearHora(horarios[0])} ({horarios.Count} horario(s)). Busque un socio para asignar.";
                lblMensaje.ForeColor = UiTheme.Primario;
                ActualizarEstadoFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No se pudo buscar la próxima fecha disponible.{Environment.NewLine}{Environment.NewLine}Detalle: {ex.Message}",
                    "Turnos nutrición",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void AsegurarFechaNoPasada()
        {
            if (dtpFecha.Value.Date >= DateTime.Today)
            {
                return;
            }

            dtpFecha.ValueChanged -= OnFechaCambiada;
            dtpFecha.Value = DateTime.Today;
            dtpFecha.ValueChanged += OnFechaCambiada;
        }

        private void PoblarHorariosDisponibles(DateTime fecha, IReadOnlyList<TimeSpan> horarios)
        {
            _turnosDisponibles = horarios
                .Select(h => new TurnoDisponibleViewModel
                {
                    Fecha = fecha,
                    Hora = h
                })
                .ToList();

            cbHorasDisponibles.Items.Clear();
            foreach (var turno in _turnosDisponibles)
            {
                cbHorasDisponibles.Items.Add(new TurnoDisponibleItem(turno));
            }

            if (cbHorasDisponibles.Items.Count > 0)
            {
                cbHorasDisponibles.SelectedIndex = 0;
            }
            else
            {
                cbHorasDisponibles.Text = string.Empty;
            }

            lblFechaDisponibles.Text = _turnosDisponibles.Count == 0
                ? $"Sin horarios libres el {fecha:dd/MM/yyyy}"
                : $"{_turnosDisponibles.Count} horario(s) libre(s) el {fecha:dd/MM/yyyy}";
        }

        private void CargarTurnosDisponibles()
        {
            if (_cargandoHorarios)
            {
                return;
            }

            if (cbNutricionistas.SelectedItem is not NutricionistaItem nutricionistaItem)
            {
                cbHorasDisponibles.Items.Clear();
                cbHorasDisponibles.Text = string.Empty;
                lblFechaDisponibles.Text = "Turnos disponibles para: -";
                return;
            }

            _cargandoHorarios = true;
            try
            {
                AsegurarFechaNoPasada();
                var fechaSeleccionada = dtpFecha.Value.Date;
                var horarios = _turnoDao.ObtenerHorariosDisponibles(fechaSeleccionada, nutricionistaItem.Id).ToList();
                PoblarHorariosDisponibles(fechaSeleccionada, horarios);
            }
            catch (Exception ex)
            {
                cbHorasDisponibles.Items.Clear();
                cbHorasDisponibles.Text = string.Empty;
                lblFechaDisponibles.Text = "Turnos disponibles para: -";
                System.Diagnostics.Debug.WriteLine($"CargarTurnosDisponibles: {ex}");
            }
            finally
            {
                _cargandoHorarios = false;
                ActualizarEstadoFormulario();
            }
        }

        private void AsignarTurno()
        {
            if (_socioSeleccionado is null)
            {
                MessageBox.Show("Busque un socio antes de asignar el turno.", "Turnos nutrición", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbNutricionistas.SelectedItem is not NutricionistaItem nutricionistaItem)
            {
                MessageBox.Show("Seleccione un nutricionista.", "Turnos nutrición", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbHorasDisponibles.SelectedItem is not TurnoDisponibleItem turnoItem)
            {
                MessageBox.Show("Seleccione una hora disponible.", "Turnos nutrición", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var fecha = dtpFecha.Value.Date;
                var nutricionistaId = nutricionistaItem.Id;
                var hora = turnoItem.Turno.Hora;

                if (_turnoDao.ExisteConflictoHorario(nutricionistaId, fecha, hora))
                {
                    MessageBox.Show(
                        "E1: El horario seleccionado ya no está disponible. Elija otra hora o fecha.",
                        "Turnos nutrición",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    CargarTurnosDisponibles();
                    ActualizarEstadoFormulario();
                    return;
                }

                if (_turnoDao.Crear(_socioSeleccionado.IdSocio, nutricionistaId, fecha, hora, "CONFIRMADO", out _))
                {
                    MessageBox.Show(
                        "Turno asignado. Selecciónelo en la grilla para registrar la consulta y actualizar la ficha médica.",
                        "Turnos nutrición",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    CargarTurnosSocio();
                    CargarTurnosDisponibles();
                    ActualizarEstadoFormulario();
                    return;
                }

                MessageBox.Show("No se pudo asignar el turno. Intente nuevamente.", "Turnos nutrición", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                MessageBox.Show("Ocurrió un error al asignar el turno. Verifique la conexión.", "Turnos nutrición", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarFichaMedica()
        {
            if (_socioSeleccionado is null)
            {
                _fichaSeleccionada = null;
                lblFichaMedica.Text = "Ficha médica: -";
                return;
            }

            try
            {
                _fichaSeleccionada = _fichaDao.ObtenerPorSocio(_socioSeleccionado.IdSocio);
                lblFichaMedica.Text = _fichaSeleccionada is null
                    ? "Ficha médica: no registrada (se creará al guardar la consulta)"
                    : $"Ficha médica: Carga actual \"{_fichaSeleccionada.Value.CargaPermitida}\" — Peso {_fichaSeleccionada.Value.Peso} kg, Altura {_fichaSeleccionada.Value.Altura} m";
                PoblarFormularioConsulta();
            }
            catch
            {
                lblFichaMedica.Text = "Ficha médica: error al cargar";
            }
        }

        private void PoblarFormularioConsulta()
        {
            if (_fichaSeleccionada is null)
            {
                numPeso.Value = 0;
                numAltura.Value = 0;
                txtAlergias.Clear();
                txtMedicacion.Clear();
                txtObservacionesFicha.Clear();
                cbCargaPermitida.Text = string.Empty;
                return;
            }

            var ficha = _fichaSeleccionada.Value;
            numPeso.Value = Math.Min(numPeso.Maximum, Math.Max(numPeso.Minimum, ficha.Peso));
            numAltura.Value = Math.Min(numAltura.Maximum, Math.Max(numAltura.Minimum, ficha.Altura));
            txtAlergias.Text = ficha.Alergias;
            txtMedicacion.Text = ficha.Medicacion;
            txtObservacionesFicha.Text = ficha.Observaciones;
            cbCargaPermitida.Text = ficha.CargaPermitida;
        }

        private void LimpiarConsulta()
        {
            _fichaSeleccionada = null;
            _turnoConsultaSeleccionado = null;
            PoblarFormularioConsulta();
            ActualizarEstadoConsulta();
        }

        private void SeleccionarTurnoParaConsulta()
        {
            _turnoConsultaSeleccionado = dgvTurnosSocio.CurrentRow?.DataBoundItem as TurnoSocioViewModel;
            ActualizarEstadoConsulta();
        }

        private void ActualizarEstadoConsulta()
        {
            if (_socioSeleccionado is null)
            {
                btnGuardarConsulta.Enabled = false;
                lblConsultaMensaje.Text = "Busque un socio para actualizar la ficha médica.";
                lblConsultaMensaje.ForeColor = UiTheme.TextoSecundario;
                return;
            }

            if (_turnoConsultaSeleccionado is { } turno
                && turno.Estado.Equals("CANCELADO", StringComparison.OrdinalIgnoreCase))
            {
                btnGuardarConsulta.Enabled = false;
                lblConsultaMensaje.Text = "No se puede registrar consulta en un turno cancelado.";
                lblConsultaMensaje.ForeColor = UiTheme.Error;
                return;
            }

            btnGuardarConsulta.Enabled = true;

            if (_turnoConsultaSeleccionado is { } turnoSeleccionado)
            {
                lblConsultaMensaje.Text = turnoSeleccionado.Estado.Equals("ATENDIDO", StringComparison.OrdinalIgnoreCase)
                    ? $"Turno {turnoSeleccionado.Fecha:dd/MM/yyyy} ya atendido. Puede actualizar la ficha."
                    : $"Turno {turnoSeleccionado.Fecha:dd/MM/yyyy} {FormatearHora(turnoSeleccionado.Hora)} — actualice la ficha y guarde.";
                lblConsultaMensaje.ForeColor = UiTheme.Primario;
                return;
            }

            lblConsultaMensaje.Text = "Actualice la ficha médica del socio y pulse Guardar ficha.";
            lblConsultaMensaje.ForeColor = UiTheme.TextoSecundario;
        }

        private void GuardarConsulta()
        {
            if (_socioSeleccionado is null)
            {
                MessageBox.Show("Busque un socio antes de registrar la consulta.", "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numPeso.Value <= 0 || numAltura.Value <= 0)
            {
                MessageBox.Show("Ingrese peso y altura válidos.", "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cargaPermitida = cbCargaPermitida.Text.Trim();
            if (string.IsNullOrEmpty(cargaPermitida))
            {
                MessageBox.Show("Defina la carga de actividad física permitida.", "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbCargaPermitida.Focus();
                return;
            }

            var peso = numPeso.Value;
            var altura = numAltura.Value;
            var alergias = txtAlergias.Text.Trim();
            var medicacion = txtMedicacion.Text.Trim();
            var observaciones = txtObservacionesFicha.Text.Trim();

            try
            {
                var guardado = false;
                if (_fichaSeleccionada is null)
                {
                    guardado = _fichaDao.Crear(
                        _socioSeleccionado.IdSocio,
                        peso,
                        altura,
                        alergias,
                        medicacion,
                        observaciones,
                        cargaPermitida,
                        out _);
                }
                else
                {
                    guardado = _fichaDao.Actualizar(
                        _fichaSeleccionada.Value.Id,
                        peso,
                        altura,
                        alergias,
                        medicacion,
                        observaciones,
                        cargaPermitida);
                }

                if (!guardado)
                {
                    MessageBox.Show("No se pudo actualizar la ficha médica.", "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_turnoConsultaSeleccionado is { } turno
                    && !turno.Estado.Equals("CANCELADO", StringComparison.OrdinalIgnoreCase)
                    && !turno.Estado.Equals("ATENDIDO", StringComparison.OrdinalIgnoreCase)
                    && _turnoDao.Actualizar(turno.Id, turno.Fecha, turno.Hora, "ATENDIDO"))
                {
                    turno.Estado = "ATENDIDO";
                }

                CargarFichaMedica();
                CargarTurnosSocio();
                ActualizarEstadoConsulta();

                MessageBox.Show(
                    "Ficha médica actualizada correctamente.",
                    "Consulta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Ocurrió un error al guardar la consulta. Verifique la conexión.", "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarEstadoFormulario()
        {
            if (_socioSeleccionado is null)
            {
                btnAsignarTurno.Enabled = false;
                if (cbHorasDisponibles.Items.Count > 0)
                {
                    lblMensaje.Text = $"{cbHorasDisponibles.Items.Count} horario(s) disponible(s). Busque un socio para continuar.";
                    lblMensaje.ForeColor = UiTheme.Primario;
                }
                else if (cbNutricionistas.SelectedItem is NutricionistaItem)
                {
                    lblMensaje.Text = "E1: Sin horarios libres en esta fecha. Use «Buscar próxima fecha» o elija otra.";
                    lblMensaje.ForeColor = Color.DarkRed;
                }
                else
                {
                    lblMensaje.Text = "Seleccione nutricionista y fecha para ver horarios disponibles.";
                    lblMensaje.ForeColor = UiTheme.TextoSecundario;
                }

                return;
            }

            if (!(_socioSeleccionado.EstadoCuota.Equals("AL_DIA", StringComparison.OrdinalIgnoreCase)
                  || _socioSeleccionado.EstadoCuota.Equals("Al día", StringComparison.OrdinalIgnoreCase)))
            {
                lblMensaje.Text = "Precondición: el socio debe estar activo (cuota al día) para asignar turnos.";
                lblMensaje.ForeColor = Color.DarkRed;
                btnAsignarTurno.Enabled = false;
                return;
            }

            if (cbNutricionistas.SelectedItem is not NutricionistaItem)
            {
                lblMensaje.Text = "Seleccione un nutricionista.";
                lblMensaje.ForeColor = Color.DarkRed;
                btnAsignarTurno.Enabled = false;
                return;
            }

            if (cbHorasDisponibles.SelectedItem is not TurnoDisponibleItem)
            {
                lblMensaje.Text = "E1: No hay horarios libres en esta fecha. Elija otra fecha.";
                lblMensaje.ForeColor = Color.DarkRed;
                btnAsignarTurno.Enabled = false;
                return;
            }

            lblMensaje.Text = "Turno listo para asignar.";
            lblMensaje.ForeColor = Color.DarkGreen;
            btnAsignarTurno.Enabled = true;
        }

        private void ConfigurarColumnasTurnosSocio()
        {
            if (dgvTurnosSocio.Columns.Count == 0)
            {
                return;
            }

            foreach (DataGridViewColumn columna in dgvTurnosSocio.Columns)
            {
                columna.Visible = columna.Name is not "Id";
                columna.ReadOnly = true;
            }

            if (dgvTurnosSocio.Columns.Contains("Nutricionista"))
            {
                dgvTurnosSocio.Columns["Nutricionista"].HeaderText = "Nutricionista";
                dgvTurnosSocio.Columns["Nutricionista"].FillWeight = 24;
            }

            if (dgvTurnosSocio.Columns.Contains("Fecha"))
            {
                var columnaFecha = dgvTurnosSocio.Columns["Fecha"];
                columnaFecha.HeaderText = "Fecha";
                columnaFecha.DefaultCellStyle.Format = "dd/MM/yyyy";
                columnaFecha.FillWeight = 18;
            }

            if (dgvTurnosSocio.Columns.Contains("Hora"))
            {
                var columnaHora = dgvTurnosSocio.Columns["Hora"];
                columnaHora.HeaderText = "Hora";
                columnaHora.FillWeight = 14;
            }

            if (dgvTurnosSocio.Columns.Contains("Estado"))
            {
                var columnaEstado = dgvTurnosSocio.Columns["Estado"];
                columnaEstado.HeaderText = "Estado";
                columnaEstado.FillWeight = 14;
            }

            dgvTurnosSocio.AutoResizeColumns();
        }

        private static DataGridView CrearGrilla()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                MultiSelect = false
            };
        }

        private sealed class NutricionistaItem
        {
            public int Id { get; }

            public (int IdNutricionista, string DNI, string Nombre, string Apellido, string Telefono, string Email, string Matricula) Nutricionista { get; }

            public NutricionistaItem((int Id, string DNI, string Nombre, string Apellido, string Telefono, string Email, string Matricula) nutricionista)
            {
                Id = nutricionista.Id;
                Nutricionista = nutricionista;
            }

            public override string ToString() => $"{Nutricionista.Nombre} {Nutricionista.Apellido} ({Nutricionista.Matricula})";
        }

        private static string FormatearHora(TimeSpan hora) =>
            $"{hora.Hours:D2}:{hora.Minutes:D2}";

        private sealed class TurnoDisponibleItem
        {
            public TurnoDisponibleViewModel Turno { get; }

            public TurnoDisponibleItem(TurnoDisponibleViewModel turno)
            {
                Turno = turno;
            }

            public override string ToString() => FormatearHora(Turno.Hora);
        }

        private sealed class TurnoDisponibleViewModel
        {
            public DateTime Fecha { get; set; }
            public TimeSpan Hora { get; set; }
        }

        private sealed class TurnoSocioViewModel
        {
            public int Id { get; set; }
            public string Nutricionista { get; set; } = string.Empty;
            public DateTime Fecha { get; set; }
            public TimeSpan Hora { get; set; }
            public string Estado { get; set; } = string.Empty;
        }
    }
}
