using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TP_ClubDeportivo.DAO;
using TP_ClubDeportivo.Models;

namespace TP_ClubDeportivo.Forms
{
    public class FormLiquidarHaberes : Form
    {
        private const decimal PorcentajeDescuentoEstandar = 0.10m;
        private const decimal PorcentajeDescuentoSinAsistencia = 0.20m;

        private readonly LiquidacionDAO _liquidacionDao = new();
        private readonly ProfesorDAO _profesorDao = new();
        private readonly AsistenciaDAO _asistenciaDao = new();

        private readonly ComboBox cboMes;
        private readonly NumericUpDown numAnio;
        private readonly DataGridView dgvLiquidaciones;
        private readonly Label lblResumen;
        private readonly Label lblDiaHabil;
        private readonly Label lblDetalle;
        private readonly Label lblPeriodoDestacado;
        private readonly Label lblPeriodoCodigo;
        private readonly Label lblTituloGrilla;
        private readonly Label lblBannerLiquidacion;
        private readonly Panel panelBannerLiquidacion;
        private readonly Panel panelBadgePeriodo;
        private readonly Button btnCargar;
        private readonly Button btnLiquidarMes;
        private readonly Button btnRegistrarPago;
        private readonly Button btnVerRecibo;
        private readonly Button btnCerrarBanner;
        private readonly DateTimePicker dtpFechaPago;

        private List<LiquidacionViewModel> _liquidaciones = new();

        public FormLiquidarHaberes()
        {
            Text = "Liquidar haberes (CU-08)";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1020, 640);
            MinimumSize = new Size(920, 560);
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

            void AgregarFilaSup(Control control)
            {
                var fila = layoutSuperior.RowCount;
                layoutSuperior.RowCount++;
                layoutSuperior.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                control.Dock = DockStyle.Top;
                layoutSuperior.Controls.Add(control, 0, fila);
            }

            AgregarFilaSup(new Label
            {
                Text = "Liquidación mensual de haberes",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = UiTheme.Texto,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 6)
            });

            AgregarFilaSup(new Label
            {
                Text = "Calcule liquidaciones según sueldo base, genere recibos y registre pagos (RF-11: último día hábil del mes).",
                Font = UiTheme.FuenteSubtitulo,
                ForeColor = UiTheme.TextoSecundario,
                AutoSize = true,
                MaximumSize = new Size(720, 0),
                Margin = new Padding(0, 0, 0, 12)
            });

            panelBadgePeriodo = new Panel
            {
                Size = new Size(220, 88),
                BackColor = UiTheme.PrimarioClaro,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            panelBadgePeriodo.Paint += (_, e) =>
            {
                using var pen = new Pen(UiTheme.Primario, 2);
                var rect = new Rectangle(0, 0, panelBadgePeriodo.Width - 1, panelBadgePeriodo.Height - 1);
                e.Graphics.DrawRectangle(pen, rect);
            };

            lblPeriodoDestacado = new Label
            {
                Text = "—",
                Location = new Point(12, 14),
                Size = new Size(196, 32),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = UiTheme.PrimarioOscuro,
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblPeriodoCodigo = new Label
            {
                Text = "Período —",
                Location = new Point(12, 50),
                Size = new Size(196, 22),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = UiTheme.Primario,
                TextAlign = ContentAlignment.MiddleCenter
            };

            panelBadgePeriodo.Controls.AddRange([lblPeriodoDestacado, lblPeriodoCodigo]);
            panelSuperior.Controls.Add(panelBadgePeriodo);
            panelSuperior.Resize += (_, _) =>
                panelBadgePeriodo.Location = new Point(Math.Max(24, panelSuperior.Width - panelBadgePeriodo.Width - 28), 72);

            var filaPeriodo = new TableLayoutPanel
            {
                ColumnCount = 6,
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 0, 0, 8)
            };
            filaPeriodo.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filaPeriodo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            filaPeriodo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            filaPeriodo.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filaPeriodo.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filaPeriodo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filaPeriodo.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            filaPeriodo.Controls.Add(new Label
            {
                Text = "Período a liquidar:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 10, 12, 0),
                Font = UiTheme.FuenteNormal
            }, 0, 0);

            cboMes = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = UiTheme.FuenteNormal,
                Margin = new Padding(0, 4, 8, 0)
            };
            for (var m = 1; m <= 12; m++)
            {
                cboMes.Items.Add(new MesItem(m));
            }
            cboMes.SelectedIndex = DateTime.Today.Month - 1;
            cboMes.SelectedIndexChanged += (_, _) =>
            {
                OcultarBannerLiquidacion();
                ActualizarEtiquetasPeriodo();
            };
            filaPeriodo.Controls.Add(cboMes, 1, 0);

            numAnio = new NumericUpDown
            {
                Dock = DockStyle.Fill,
                Minimum = 2020,
                Maximum = 2100,
                Value = DateTime.Today.Year,
                Margin = new Padding(0, 4, 12, 0)
            };
            numAnio.ValueChanged += (_, _) =>
            {
                OcultarBannerLiquidacion();
                ActualizarEtiquetasPeriodo();
            };
            filaPeriodo.Controls.Add(numAnio, 2, 0);

            btnCargar = new Button { Text = "Consultar período" };
            UiTheme.AjustarBotonToolbar(btnCargar);
            btnCargar.Click += (_, _) => CargarPeriodo();
            filaPeriodo.Controls.Add(btnCargar, 3, 0);

            btnLiquidarMes = new Button { Text = "Liquidar mes" };
            UiTheme.AjustarBotonToolbar(btnLiquidarMes, primario: true);
            btnLiquidarMes.Click += (_, _) => LiquidarMes();
            filaPeriodo.Controls.Add(btnLiquidarMes, 4, 0);

            AgregarFilaSup(filaPeriodo);

            lblDiaHabil = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(760, 0),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 4)
            };
            AgregarFilaSup(lblDiaHabil);

            lblResumen = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(760, 0),
                ForeColor = UiTheme.PrimarioOscuro,
                Font = UiTheme.FuenteNormal,
                Margin = new Padding(0, 0, 0, 4)
            };
            AgregarFilaSup(lblResumen);

            panelSuperior.Controls.Add(layoutSuperior);

            panelBannerLiquidacion = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.FromArgb(232, 248, 238),
                Padding = new Padding(24, 10, 24, 10),
                Visible = false
            };
            panelBannerLiquidacion.Paint += (_, e) =>
            {
                using var pen = new Pen(UiTheme.Exito, 2);
                e.Graphics.DrawLine(pen, 0, panelBannerLiquidacion.Height - 1, panelBannerLiquidacion.Width, panelBannerLiquidacion.Height - 1);
            };

            lblBannerLiquidacion = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UiTheme.Exito,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnCerrarBanner = new Button
            {
                Text = "×",
                Dock = DockStyle.Right,
                Width = 36,
                FlatStyle = FlatStyle.Flat,
                ForeColor = UiTheme.Exito,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCerrarBanner.FlatAppearance.BorderSize = 0;
            btnCerrarBanner.Click += (_, _) => OcultarBannerLiquidacion();

            panelBannerLiquidacion.Controls.Add(lblBannerLiquidacion);
            panelBannerLiquidacion.Controls.Add(btnCerrarBanner);

            dgvLiquidaciones = new DataGridView
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
            dgvLiquidaciones.SelectionChanged += (_, _) => ActualizarDetalleSeleccion();
            dgvLiquidaciones.DataBindingComplete += (_, _) => ConfigurarColumnas();

            lblTituloGrilla = new Label
            {
                Dock = DockStyle.Top,
                Height = 32,
                Padding = new Padding(24, 8, 24, 0),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = UiTheme.Texto,
                BackColor = UiTheme.Fondo,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var panelGrilla = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 4, 24, 8),
                BackColor = UiTheme.Fondo
            };
            panelGrilla.Controls.Add(dgvLiquidaciones);

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

            lblDetalle = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(900, 0),
                ForeColor = UiTheme.TextoSecundario,
                Font = new Font("Segoe UI", 9F),
                Margin = new Padding(0, 0, 0, 10)
            };
            layoutAccion.Controls.Add(lblDetalle, 0, 0);
            layoutAccion.RowCount = 1;
            layoutAccion.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var filaPago = new TableLayoutPanel
            {
                ColumnCount = 4,
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            filaPago.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filaPago.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            filaPago.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filaPago.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filaPago.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            filaPago.Controls.Add(new Label
            {
                Text = "Fecha de pago:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 10, 8, 0),
                Font = UiTheme.FuenteNormal
            }, 0, 0);

            dtpFechaPago = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Fill,
                Value = DateTime.Today,
                Margin = new Padding(0, 4, 12, 0)
            };
            filaPago.Controls.Add(dtpFechaPago, 1, 0);

            btnRegistrarPago = new Button { Text = "Registrar pago" };
            UiTheme.AjustarBotonToolbar(btnRegistrarPago, primario: true);
            btnRegistrarPago.Click += (_, _) => RegistrarPago();
            filaPago.Controls.Add(btnRegistrarPago, 2, 0);

            btnVerRecibo = new Button { Text = "Ver recibo" };
            UiTheme.AjustarBotonToolbar(btnVerRecibo);
            btnVerRecibo.Click += (_, _) => MostrarRecibo();
            filaPago.Controls.Add(btnVerRecibo, 3, 0);

            layoutAccion.RowCount = 2;
            layoutAccion.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutAccion.Controls.Add(filaPago, 0, 1);

            panelAccion.Controls.Add(layoutAccion);

            Controls.Add(panelGrilla);
            Controls.Add(lblTituloGrilla);
            Controls.Add(panelAccion);
            Controls.Add(panelBannerLiquidacion);
            Controls.Add(panelSuperior);

            Load += (_, _) =>
            {
                if (!Permisos.ValidarAccesoAlAbrir(this, Permisos.Modulo.LiquidarHaberes))
                {
                    return;
                }

                panelBadgePeriodo.Location = new Point(panelSuperior.Width - panelBadgePeriodo.Width - 28, 72);
                ActualizarAvisoDiaHabil();
                ActualizarEtiquetasPeriodo();
                CargarPeriodo();
            };
        }

        private int MesSeleccionado =>
            cboMes.SelectedItem is MesItem mes ? mes.Numero : DateTime.Today.Month;

        private void ActualizarEtiquetasPeriodo()
        {
            var mes = MesSeleccionado;
            var anio = (int)numAnio.Value;
            var nombreMes = ObtenerNombreMes(mes);

            lblPeriodoDestacado.Text = $"{nombreMes} {anio}";
            lblPeriodoCodigo.Text = $"Período {mes:00}/{anio}";
            lblTituloGrilla.Text = $"Liquidaciones del período — {nombreMes} {anio} ({mes:00}/{anio})";
        }

        private void MostrarBannerLiquidacion(int mes, int anio, int creadas, string? detalleProfesores = null)
        {
            var nombreMes = ObtenerNombreMes(mes);
            var texto =
                $"✓ Período liquidado: {nombreMes} {anio} ({mes:00}/{anio}) — se generaron {creadas} recibo(s) nuevo(s). " +
                "La grilla muestra todas las liquidaciones de ese mes.";

            if (!string.IsNullOrEmpty(detalleProfesores))
            {
                texto += $" {detalleProfesores}";
            }

            lblBannerLiquidacion.Text = texto;
            panelBannerLiquidacion.Visible = true;
            lblTituloGrilla.ForeColor = UiTheme.Exito;
        }

        private void OcultarBannerLiquidacion()
        {
            panelBannerLiquidacion.Visible = false;
            lblTituloGrilla.ForeColor = UiTheme.Texto;
        }

        private static string ObtenerNombreMes(int mes)
        {
            var nombre = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mes);
            return char.ToUpper(nombre[0]) + nombre[1..];
        }

        private void CargarPeriodo()
        {
            try
            {
                var mes = MesSeleccionado;
                var anio = (int)numAnio.Value;
                ActualizarEtiquetasPeriodo();

                _liquidaciones = _liquidacionDao.ObtenerPorPeriodo(mes, anio)
                    .Select(l => new LiquidacionViewModel
                    {
                        IdLiquidacion = l.Id,
                        ProfesorId = l.ProfesorId,
                        ProfesorNombre = l.ProfesorNombre,
                        Especialidad = l.Especialidad,
                        PeriodoTexto = $"{l.Mes:00}/{l.Anio}",
                        Mes = l.Mes,
                        Anio = l.Anio,
                        MontoBruto = l.MontoBruto,
                        Descuentos = l.Descuentos,
                        MontoNeto = l.MontoNeto,
                        FechaPago = l.FechaPago,
                        Estado = l.Estado
                    })
                    .ToList();

                dgvLiquidaciones.DataSource = null;
                dgvLiquidaciones.DataSource = _liquidaciones;

                var nombreMes = ObtenerNombreMes(mes);
                var reporte = _liquidacionDao.ObtenerReportePeriodo(mes, anio);
                if (reporte is null || reporte.Value.CantidadProfesores == 0)
                {
                    lblResumen.Text =
                        $"{nombreMes} {anio}: sin liquidaciones. Elegí el período y usá «Liquidar mes».";
                }
                else
                {
                    var r = reporte.Value;
                    lblResumen.Text =
                        $"{nombreMes} {anio} ({mes:00}/{anio}) — {r.CantidadProfesores} liquidación(es) · " +
                        $"Bruto {FormatearMonto(r.TotalBruto)} · Descuentos {FormatearMonto(r.TotalDescuentos)} · " +
                        $"Neto {FormatearMonto(r.TotalNeto)} · Pagadas: {r.PagosRealizados} · Pendientes: {r.PagosPendientes}";
                }

                ActualizarDetalleSeleccion();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar liquidaciones: {ex.Message}", "Liquidar haberes",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LiquidarMes()
        {
            if (!ValidarUltimoDiaHabil("generar las liquidaciones del mes"))
            {
                return;
            }

            var mes = MesSeleccionado;
            var anio = (int)numAnio.Value;
            var nombreMes = ObtenerNombreMes(mes);

            List<Profesor> profesores;
            try
            {
                profesores = _profesorDao.ObtenerTodos().ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudieron cargar los profesores: {ex.Message}", "Liquidar haberes",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (profesores.Count == 0)
            {
                MessageBox.Show(
                    "No hay profesores activos en el sistema.",
                    "Precondición",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var idsConLiquidacion = _liquidaciones.Select(l => l.ProfesorId).ToHashSet();
            var pendientes = profesores.Where(p => !idsConLiquidacion.Contains(p.IdProfesor)).ToList();

            if (pendientes.Count == 0)
            {
                MessageBox.Show(
                    $"Todos los profesores ya tienen liquidación para {nombreMes} {anio} ({mes:00}/{anio}).",
                    "Liquidar mes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Generar {pendientes.Count} liquidación(es) para {nombreMes} {anio}?\n\n" +
                $"Período: {mes:00}/{anio}\n" +
                "Se calculará el monto según sueldo base y asistencia del mes (E1: descuento si no hay asistencia).",
                "Confirmar liquidación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            var creadas = 0;
            var errores = new List<string>();
            var nombresCreados = new List<string>();

            foreach (var profesor in pendientes)
            {
                var (bruto, descuentos, neto, _) = CalcularMontos(profesor, mes, anio);

                if (!_liquidacionDao.Crear(
                        profesor.IdProfesor,
                        mes,
                        anio,
                        bruto,
                        descuentos,
                        neto,
                        "PENDIENTE",
                        out _))
                {
                    errores.Add($"{profesor.Nombre} {profesor.Apellido}");
                    continue;
                }

                creadas++;
                nombresCreados.Add($"{profesor.Nombre} {profesor.Apellido}");
            }

            var resumenProfesores = nombresCreados.Count > 0
                ? $"Profesores: {string.Join(", ", nombresCreados)}."
                : null;

            MostrarBannerLiquidacion(mes, anio, creadas, resumenProfesores);
            CargarPeriodo();

            if (errores.Count > 0)
            {
                MessageBox.Show(
                    $"Período {nombreMes} {anio}: no se pudo liquidar a {string.Join(", ", errores)}.",
                    "Liquidación parcial",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else if (creadas > 0)
            {
                MessageBox.Show(
                    $"Se generaron {creadas} liquidación(es) para {nombreMes} {anio} ({mes:00}/{anio}).\n\n" +
                    "Revisá el banner verde y la grilla con el mismo período.",
                    "Liquidación generada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void RegistrarPago()
        {
            if (!ValidarUltimoDiaHabil("registrar el pago de haberes"))
            {
                return;
            }

            if (dgvLiquidaciones.CurrentRow?.DataBoundItem is not LiquidacionViewModel liquidacion)
            {
                MessageBox.Show("Seleccione una liquidación de la grilla.", "Registrar pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (liquidacion.Estado == "PAGADO")
            {
                MessageBox.Show("La liquidación seleccionada ya fue pagada.", "Registrar pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Registrar pago de {FormatearMonto(liquidacion.MontoNeto)} a {liquidacion.ProfesorNombre}?\n\n" +
                $"Recibo #{liquidacion.IdLiquidacion} — Período {liquidacion.Mes:00}/{liquidacion.Anio}",
                "Confirmar pago",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (!_liquidacionDao.Pagar(liquidacion.IdLiquidacion, dtpFechaPago.Value.Date, out var mensaje))
                {
                    MessageBox.Show("No se pudo registrar el pago.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MessageBox.Show(
                    $"{mensaje}\n\n{liquidacion.ProfesorNombre} — {FormatearMonto(liquidacion.MontoNeto)} " +
                    $"({dtpFechaPago.Value:dd/MM/yyyy})",
                    "Pago registrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CargarPeriodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Registrar pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarRecibo()
        {
            if (dgvLiquidaciones.CurrentRow?.DataBoundItem is not LiquidacionViewModel liquidacion)
            {
                MessageBox.Show("Seleccione una liquidación para ver el recibo.", "Recibo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var recibo = new StringBuilder();
            recibo.AppendLine("CLUB DEPORTIVO — RECIBO DE LIQUIDACIÓN");
            recibo.AppendLine(new string('─', 42));
            recibo.AppendLine($"Nº recibo:     {liquidacion.IdLiquidacion}");
            recibo.AppendLine($"Profesor:      {liquidacion.ProfesorNombre}");
            recibo.AppendLine($"Especialidad:  {liquidacion.Especialidad}");
            recibo.AppendLine($"Período:       {liquidacion.Mes:00}/{liquidacion.Anio}");
            recibo.AppendLine(new string('─', 42));
            recibo.AppendLine($"Monto bruto:   {FormatearMonto(liquidacion.MontoBruto)}");
            recibo.AppendLine($"Descuentos:    {FormatearMonto(liquidacion.Descuentos)}");
            recibo.AppendLine($"Monto neto:    {FormatearMonto(liquidacion.MontoNeto)}");
            recibo.AppendLine($"Estado:        {liquidacion.Estado}");

            if (liquidacion.FechaPago.HasValue)
            {
                recibo.AppendLine($"Fecha de pago: {liquidacion.FechaPago.Value:dd/MM/yyyy}");
            }

            recibo.AppendLine(new string('─', 42));
            recibo.AppendLine($"Emitido: {DateTime.Now:dd/MM/yyyy HH:mm}");

            MessageBox.Show(recibo.ToString(), "Recibo de liquidación (CU-08)",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private (decimal Bruto, decimal Descuentos, decimal Neto, string Motivo) CalcularMontos(Profesor profesor, int mes, int anio)
        {
            var bruto = Math.Round((decimal)profesor.SueldoMensual, 2);
            var inicio = new DateTime(anio, mes, 1);
            var fin = inicio.AddMonths(1).AddDays(-1);

            var asistenciasMes = _asistenciaDao.ObtenerPorProfesor(profesor.IdProfesor)
                .Where(a => a.Fecha.Date >= inicio && a.Fecha.Date <= fin)
                .ToList();

            decimal descuentos;
            string motivo;

            if (asistenciasMes.Count == 0)
            {
                descuentos = Math.Round(bruto * PorcentajeDescuentoSinAsistencia, 2);
                motivo = "E1 — Sin asistencia registrada en el mes (20%)";
            }
            else if (!asistenciasMes.Exists(a => a.Presente))
            {
                descuentos = Math.Round(bruto * PorcentajeDescuentoSinAsistencia, 2);
                motivo = "E1 — Sin días presentes en el mes (20%)";
            }
            else
            {
                descuentos = Math.Round(bruto * PorcentajeDescuentoEstandar, 2);
                motivo = "Descuento estándar por asistencia (10%)";
            }

            return (bruto, descuentos, bruto - descuentos, motivo);
        }

        private bool ValidarUltimoDiaHabil(string accion)
        {
            if (EsUltimoDiaHabilDelMes(DateTime.Today))
            {
                return true;
            }

            var ultimoHabil = ObtenerUltimoDiaHabilDelMes(DateTime.Today);
            var continuar = MessageBox.Show(
                $"RF-11: la liquidación de haberes debe realizarse el último día hábil del mes.\n\n" +
                $"Hoy: {DateTime.Today:dd/MM/yyyy} ({DateTime.Today:dddd})\n" +
                $"Último día hábil de este mes: {ultimoHabil:dd/MM/yyyy}\n\n" +
                $"¿Desea {accion} de todos modos? (solo para pruebas)",
                "Fuera del último día hábil",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            return continuar == DialogResult.Yes;
        }

        private void ActualizarAvisoDiaHabil()
        {
            if (EsUltimoDiaHabilDelMes(DateTime.Today))
            {
                lblDiaHabil.Text = "✓ Hoy es el último día hábil del mes — puede liquidar y pagar haberes.";
                lblDiaHabil.ForeColor = Color.FromArgb(30, 120, 60);
            }
            else
            {
                var ultimo = ObtenerUltimoDiaHabilDelMes(DateTime.Today);
                lblDiaHabil.Text =
                    $"RF-11: hoy no es el último día hábil. Próximo cierre: {ultimo:dd/MM/yyyy} ({ultimo:dddd}).";
                lblDiaHabil.ForeColor = UiTheme.Error;
            }
        }

        private void ActualizarDetalleSeleccion()
        {
            btnRegistrarPago.Enabled = false;
            btnVerRecibo.Enabled = false;
            lblDetalle.Text = string.Empty;

            if (dgvLiquidaciones.CurrentRow?.DataBoundItem is not LiquidacionViewModel liquidacion)
            {
                return;
            }

            btnVerRecibo.Enabled = true;
            btnRegistrarPago.Enabled = liquidacion.Estado != "PAGADO";

            if (liquidacion.Estado == "PAGADO")
            {
                lblDetalle.ForeColor = UiTheme.TextoSecundario;
                lblDetalle.Text =
                    $"Recibo #{liquidacion.IdLiquidacion} pagado el {liquidacion.FechaPago:dd/MM/yyyy} — " +
                    $"neto {FormatearMonto(liquidacion.MontoNeto)}.";
            }
            else
            {
                lblDetalle.ForeColor = UiTheme.Primario;
                lblDetalle.Text =
                    $"Recibo #{liquidacion.IdLiquidacion} pendiente — {liquidacion.ProfesorNombre}: " +
                    $"bruto {FormatearMonto(liquidacion.MontoBruto)}, descuentos {FormatearMonto(liquidacion.Descuentos)}, " +
                    $"a pagar {FormatearMonto(liquidacion.MontoNeto)}.";
            }
        }

        private void ConfigurarColumnas()
        {
            if (dgvLiquidaciones.Columns.Count == 0)
            {
                return;
            }

            OcultarColumna("ProfesorId");
            OcultarColumna("Mes");
            OcultarColumna("Anio");

            var titulos = new Dictionary<string, string>
            {
                ["IdLiquidacion"] = "Nº recibo",
                ["PeriodoTexto"] = "Período",
                ["ProfesorNombre"] = "Profesor",
                ["Especialidad"] = "Especialidad",
                ["MontoBruto"] = "Bruto ($)",
                ["Descuentos"] = "Descuentos ($)",
                ["MontoNeto"] = "Neto ($)",
                ["Estado"] = "Estado",
                ["FechaPago"] = "Fecha pago"
            };

            foreach (var par in titulos)
            {
                if (dgvLiquidaciones.Columns.Contains(par.Key))
                {
                    dgvLiquidaciones.Columns[par.Key]!.HeaderText = par.Value;
                }
            }

            foreach (var col in new[] { "MontoBruto", "Descuentos", "MontoNeto" })
            {
                if (dgvLiquidaciones.Columns.Contains(col))
                {
                    dgvLiquidaciones.Columns[col]!.DefaultCellStyle.Format = "N2";
                    dgvLiquidaciones.Columns[col]!.DefaultCellStyle.FormatProvider = CultureInfo.CurrentCulture;
                }
            }

            if (dgvLiquidaciones.Columns.Contains("FechaPago"))
            {
                dgvLiquidaciones.Columns["FechaPago"]!.DefaultCellStyle.Format = "dd/MM/yyyy";
            }

            foreach (DataGridViewRow row in dgvLiquidaciones.Rows)
            {
                if (row.DataBoundItem is not LiquidacionViewModel item)
                {
                    continue;
                }

                if (item.Estado == "PAGADO")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(235, 248, 240);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(30, 100, 55);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 230);
                    row.DefaultCellStyle.ForeColor = UiTheme.Texto;
                }
            }
        }

        private static bool EsUltimoDiaHabilDelMes(DateTime fecha) =>
            fecha.Date == ObtenerUltimoDiaHabilDelMes(fecha).Date;

        private static DateTime ObtenerUltimoDiaHabilDelMes(DateTime fecha)
        {
            var ultimo = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
            while (ultimo.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                ultimo = ultimo.AddDays(-1);
            }

            return ultimo;
        }

        private static string FormatearMonto(decimal monto) =>
            monto.ToString("C2", CultureInfo.CurrentCulture);

        private void OcultarColumna(string nombre)
        {
            if (dgvLiquidaciones.Columns.Contains(nombre))
            {
                dgvLiquidaciones.Columns[nombre]!.Visible = false;
            }
        }

        private sealed class MesItem(int numero)
        {
            public int Numero { get; } = numero;

            public override string ToString()
            {
                var nombre = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Numero);
                return char.ToUpper(nombre[0]) + nombre[1..];
            }
        }

        private sealed class LiquidacionViewModel
        {
            public int IdLiquidacion { get; init; }
            public int ProfesorId { get; init; }
            public string ProfesorNombre { get; init; } = string.Empty;
            public string Especialidad { get; init; } = string.Empty;
            public string PeriodoTexto { get; init; } = string.Empty;
            public int Mes { get; init; }
            public int Anio { get; init; }
            public decimal MontoBruto { get; init; }
            public decimal Descuentos { get; init; }
            public decimal MontoNeto { get; init; }
            public DateTime? FechaPago { get; init; }
            public string Estado { get; init; } = string.Empty;
        }
    }
}
