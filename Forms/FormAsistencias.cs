using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TP_ClubDeportivo.DAO;
using TP_ClubDeportivo.Models;

namespace TP_ClubDeportivo.Forms
{
    public class FormAsistencias : Form
    {
        private readonly ProfesorDAO _profesorDao = new();
        private readonly AsistenciaDAO _asistenciaDao = new();

        private readonly ComboBox cbProfesores;
        private readonly DateTimePicker dtpFecha;
        private readonly DataGridView dgvAsistencias;
        private readonly TextBox txtFirma;
        private readonly CheckBox chkPresente;
        private readonly Button btnBuscar;
        private readonly Button btnFirmar;
        private readonly Label lblMensaje;

        private List<AsistenciaViewModel> _asistencias = new();

        public FormAsistencias()
        {
            Text = "Firmar asistencia (CU-05)";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(980, 620);
            MinimumSize = new Size(900, 520);
            Font = UiTheme.FuenteNormal;
            BackColor = UiTheme.Fondo;

            var panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = UiTheme.Tarjeta,
                Padding = new Padding(24)
            };

            var layoutSuperior = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1
            };
            layoutSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            layoutSuperior.Controls.Add(new Label
            {
                Text = "Firmar asistencia de profesor",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = UiTheme.Texto,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 6)
            }, 0, 0);
            layoutSuperior.RowCount = 1;
            layoutSuperior.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            layoutSuperior.RowCount = 2;
            layoutSuperior.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutSuperior.Controls.Add(new Label
            {
                Text = "Seleccione un profesor y una fecha. Luego registre o actualice la firma de su asistencia.",
                Font = UiTheme.FuenteSubtitulo,
                ForeColor = UiTheme.TextoSecundario,
                AutoSize = true,
                MaximumSize = new Size(900, 0),
                Margin = new Padding(0, 0, 0, 12)
            }, 0, 1);

            var filaFiltros = new TableLayoutPanel
            {
                ColumnCount = 3,
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0)
            };
            filaFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filaFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filaFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filaFiltros.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));

            cbProfesores = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 12, 0)
            };

            dtpFecha = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 150,
                Value = DateTime.Today,
                MaxDate = DateTime.Today,
                Margin = new Padding(0, 0, 12, 0)
            };
            dtpFecha.ValueChanged += (_, _) => CargarAsistencias();

            btnBuscar = new Button { Text = "Buscar asistencia" };
            UiTheme.AjustarBotonToolbar(btnBuscar, primario: true);
            btnBuscar.Click += (_, _) => CargarAsistencias();

            filaFiltros.Controls.Add(cbProfesores, 0, 0);
            filaFiltros.Controls.Add(dtpFecha, 1, 0);
            filaFiltros.Controls.Add(btnBuscar, 2, 0);

            layoutSuperior.RowCount = 3;
            layoutSuperior.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutSuperior.Controls.Add(filaFiltros, 0, 2);
            panelSuperior.Controls.Add(layoutSuperior);

            dgvAsistencias = CrearGrilla();
            dgvAsistencias.SelectionChanged += (_, _) => ActualizarFormularioDesdeSeleccion();
            dgvAsistencias.DataBindingComplete += (_, _) => ConfigurarColumnasAsistencias();

            var panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24)
            };
            panelContenido.Controls.Add(dgvAsistencias);

            var panelAccion = new Panel
            {
                Dock = DockStyle.Bottom,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = UiTheme.Tarjeta,
                Padding = new Padding(24)
            };

            var layoutAccion = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1
            };
            layoutAccion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            layoutAccion.Controls.Add(new Label
            {
                Text = "Firma:",
                AutoSize = true,
                Font = UiTheme.FuenteNormal,
                ForeColor = UiTheme.Texto,
                Margin = new Padding(0, 0, 0, 6)
            }, 0, 0);
            layoutAccion.RowCount = 1;
            layoutAccion.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            txtFirma = new TextBox { Dock = DockStyle.Top, Height = 32, Margin = new Padding(0, 0, 0, 10) };
            UiTheme.AplicarCampo(txtFirma);
            txtFirma.TextChanged += (_, _) => ActualizarBotonFirmar();
            layoutAccion.RowCount = 2;
            layoutAccion.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutAccion.Controls.Add(txtFirma, 0, 1);

            chkPresente = new CheckBox
            {
                Text = "Presente",
                AutoSize = true,
                Checked = true,
                ForeColor = UiTheme.Texto,
                Margin = new Padding(0, 0, 0, 10)
            };
            layoutAccion.RowCount = 3;
            layoutAccion.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutAccion.Controls.Add(chkPresente, 0, 2);

            var filaBotones = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            filaBotones.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filaBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filaBotones.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            btnFirmar = new Button { Text = "Registrar / firmar" };
            UiTheme.AjustarBotonToolbar(btnFirmar, primario: true);
            btnFirmar.Click += (_, _) => GuardarFirma();

            lblMensaje = new Label
            {
                Text = string.Empty,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(12, 10, 0, 0),
                ForeColor = UiTheme.TextoSecundario,
                Font = new Font("Segoe UI", 9F)
            };

            filaBotones.Controls.Add(btnFirmar, 0, 0);
            filaBotones.Controls.Add(lblMensaje, 1, 0);
            layoutAccion.RowCount = 4;
            layoutAccion.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutAccion.Controls.Add(filaBotones, 0, 3);

            panelAccion.Controls.Add(layoutAccion);

            Controls.Add(panelContenido);
            Controls.Add(panelAccion);
            Controls.Add(panelSuperior);

            Load += (_, _) =>
            {
                if (!Permisos.ValidarAccesoAlAbrir(this, Permisos.Modulo.FirmarAsistencia))
                {
                    return;
                }

                CargarProfesores();
            };
        }

        private void CargarProfesores()
        {
            try
            {
                var profesores = _profesorDao.ObtenerTodos().ToList();
                cbProfesores.Items.Clear();

                foreach (var profesor in profesores)
                {
                    cbProfesores.Items.Add(new ProfesorItem(profesor));
                }

                if (cbProfesores.Items.Count > 0)
                {
                    cbProfesores.SelectedIndex = 0;
                }

                CargarAsistencias();
            }
            catch
            {
                MessageBox.Show("No se pudieron cargar los profesores. Verifique la conexión a la base de datos.", "Firmar asistencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarAsistencias()
        {
            if (cbProfesores.SelectedItem is not ProfesorItem profesorItem)
            {
                return;
            }

            try
            {
                var fechaSeleccionada = dtpFecha.Value.Date;
                var asistenciasProfesor = _asistenciaDao.ObtenerPorProfesor(profesorItem.Profesor.IdProfesor);

                _asistencias = asistenciasProfesor
                    .Where(a => a.Fecha.Date == fechaSeleccionada)
                    .Select(a => new AsistenciaViewModel
                    {
                        Id = a.Id,
                        ProfesorId = a.ProfesorId,
                        Fecha = a.Fecha,
                        Presente = a.Presente,
                        Firma = a.Firma
                    })
                    .ToList();

                dgvAsistencias.DataSource = _asistencias;
                var asistenciaHoy = _asistencias.FirstOrDefault();
                var esHoy = fechaSeleccionada == DateTime.Today;

                if (_asistencias.Any() && esHoy)
                {
                    lblMensaje.Text = $"E1: Ya firmó hoy. Asistencia registrada para {profesorItem}.";
                    btnFirmar.Text = "Asistencia registrada";
                    btnFirmar.Enabled = false;
                    txtFirma.Text = asistenciaHoy?.Firma ?? string.Empty;
                    chkPresente.Checked = asistenciaHoy?.Presente ?? true;
                    txtFirma.Enabled = false;
                    chkPresente.Enabled = false;
                }
                else
                {
                    lblMensaje.Text = _asistencias.Any()
                        ? $"{_asistencias.Count} registro(s) encontrado(s) para {profesorItem}."
                        : "No hay registro para esta fecha. Puede completar la firma y presionar Registrar.";
                    btnFirmar.Text = _asistencias.Any() ? "Actualizar firma" : "Registrar asistencia";
                    btnFirmar.Enabled = !string.IsNullOrWhiteSpace(txtFirma.Text);
                    txtFirma.Enabled = true;
                    chkPresente.Enabled = true;

                    if (!_asistencias.Any())
                    {
                        chkPresente.Checked = true;
                        txtFirma.Text = string.Empty;
                    }
                }

                ActualizarBotonFirmar();
            }
            catch
            {
                MessageBox.Show("No se pudieron cargar las asistencias. Verifique la conexión a la base de datos.", "Firmar asistencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GuardarFirma()
        {
            if (cbProfesores.SelectedItem is not ProfesorItem profesorItem)
            {
                MessageBox.Show("Seleccione un profesor antes de firmar.", "Firmar asistencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var firma = txtFirma.Text.Trim();
            if (string.IsNullOrWhiteSpace(firma))
            {
                MessageBox.Show("Ingrese la firma antes de continuar.", "Firmar asistencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var presente = chkPresente.Checked;
            var fecha = dtpFecha.Value.Date;

            try
            {
                var existeHoy = dtpFecha.Value.Date == DateTime.Today && _asistencias.Any();
                if (existeHoy)
                {
                    MessageBox.Show("E1: Ya registró asistencia hoy. No se puede guardar otra firma para el mismo día.", "Firmar asistencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (dgvAsistencias.CurrentRow?.DataBoundItem is AsistenciaViewModel asistencia && asistencia.Id > 0)
                {
                    if (_asistenciaDao.Actualizar(asistencia.Id, presente, firma))
                    {
                        MessageBox.Show("Firma actualizada correctamente.", "Firmar asistencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarAsistencias();
                        return;
                    }
                }
                else
                {
                    if (_asistenciaDao.Registrar(profesorItem.Profesor.IdProfesor, fecha, presente, firma, out _))
                    {
                        MessageBox.Show("Asistencia registrada y firmada correctamente.", "Firmar asistencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarAsistencias();
                        return;
                    }
                }

                MessageBox.Show("No se pudo guardar la firma. Intente nuevamente.", "Firmar asistencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                MessageBox.Show("Ocurrió un error al guardar la firma. Verifique la conexión y vuelva a intentar.", "Firmar asistencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarFormularioDesdeSeleccion()
        {
            if (dgvAsistencias.CurrentRow?.DataBoundItem is AsistenciaViewModel asistencia)
            {
                chkPresente.Checked = asistencia.Presente;
                txtFirma.Text = asistencia.Firma;
                btnFirmar.Text = "Actualizar firma";
            }
            else
            {
                btnFirmar.Text = _asistencias.Any() ? "Actualizar firma" : "Registrar asistencia";
            }

            ActualizarBotonFirmar();
        }

        private void ActualizarBotonFirmar()
        {
            if (!txtFirma.Enabled)
            {
                btnFirmar.Enabled = false;
                return;
            }

            btnFirmar.Enabled = !string.IsNullOrWhiteSpace(txtFirma.Text);
        }

        private void ConfigurarColumnasAsistencias()
        {
            if (dgvAsistencias.Columns.Count == 0)
            {
                return;
            }

            foreach (DataGridViewColumn columna in dgvAsistencias.Columns)
            {
                columna.Visible = columna.Name is not ("Id" or "ProfesorId");
                columna.ReadOnly = true;
            }

            var columnaFecha = dgvAsistencias.Columns[nameof(AsistenciaViewModel.Fecha)];
            columnaFecha.HeaderText = "Fecha";
            columnaFecha.DefaultCellStyle.Format = "dd/MM/yyyy";
            columnaFecha.FillWeight = 20;

            var columnaPresente = dgvAsistencias.Columns[nameof(AsistenciaViewModel.Presente)];
            columnaPresente.HeaderText = "Presente";
            columnaPresente.FillWeight = 12;

            var columnaFirma = dgvAsistencias.Columns[nameof(AsistenciaViewModel.Firma)];
            columnaFirma.HeaderText = "Firma";
            columnaFirma.FillWeight = 30;

            dgvAsistencias.AutoResizeColumns();
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

        private sealed class ProfesorItem
        {
            public Profesor Profesor { get; }

            public ProfesorItem(Profesor profesor)
            {
                Profesor = profesor;
            }

            public override string ToString() => $"{Profesor.Nombre} {Profesor.Apellido} ({Profesor.Especialidad})";
        }

        private sealed class AsistenciaViewModel
        {
            public int Id { get; set; }
            public int ProfesorId { get; set; }
            public DateTime Fecha { get; set; }
            public bool Presente { get; set; }
            public string Firma { get; set; } = string.Empty;
        }
    }
}
