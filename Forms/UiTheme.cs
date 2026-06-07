using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TP_ClubDeportivo.Forms
{
    internal static class UiTheme
    {
        public static readonly Color Primario = Color.FromArgb(37, 99, 168);
        public static readonly Color PrimarioOscuro = Color.FromArgb(28, 76, 128);
        public static readonly Color PrimarioClaro = Color.FromArgb(232, 240, 252);
        public static readonly Color Fondo = Color.FromArgb(245, 247, 250);
        public static readonly Color Tarjeta = Color.White;
        public static readonly Color Texto = Color.FromArgb(33, 37, 41);
        public static readonly Color TextoSecundario = Color.FromArgb(108, 117, 125);
        public static readonly Color Error = Color.FromArgb(185, 28, 28);
        public static readonly Color Exito = Color.FromArgb(22, 120, 68);
        public static readonly Color Borde = Color.FromArgb(222, 226, 230);
        public static readonly Color SidebarHover = Color.FromArgb(48, 115, 185);

        public static Font FuenteTitulo => new("Segoe UI", 18F, FontStyle.Bold);
        public static Font FuenteSubtitulo => new("Segoe UI", 10F);
        public static Font FuenteNormal => new("Segoe UI", 10F);

        public static void AplicarBotonPrimario(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Primario;
            boton.ForeColor = Color.White;
            boton.Cursor = Cursors.Hand;
            boton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            boton.FlatAppearance.MouseOverBackColor = PrimarioOscuro;
        }

        public static void AplicarBotonSecundario(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 1;
            boton.FlatAppearance.BorderColor = Borde;
            boton.BackColor = Tarjeta;
            boton.ForeColor = Texto;
            boton.Cursor = Cursors.Hand;
            boton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            boton.Padding = new Padding(12, 4, 12, 4);
        }

        public static void AplicarBotonExito(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Exito;
            boton.ForeColor = Color.White;
            boton.Cursor = Cursors.Hand;
            boton.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(18, 100, 58);
            boton.Padding = new Padding(14, 4, 14, 4);
        }

        public static void AplicarCampo(TextBox campo)
        {
            campo.BorderStyle = BorderStyle.FixedSingle;
            campo.BackColor = Color.White;
            campo.Font = FuenteNormal;
        }

        public static Panel CrearTarjetaAcceso(string titulo, string descripcion, string modulo, Action alHacerClic)
        {
            var tarjeta = new Panel
            {
                Size = new Size(272, 158),
                MinimumSize = new Size(240, 150),
                BackColor = Tarjeta,
                Cursor = Cursors.Hand,
                Margin = new Padding(16, 16, 16, 16),
                Padding = new Padding(20, 18, 16, 16)
            };

            tarjeta.Paint += (_, e) =>
            {
                var rect = new Rectangle(0, 0, tarjeta.Width - 1, tarjeta.Height - 1);
                using var pen = new Pen(Borde);
                e.Graphics.DrawRectangle(pen, rect);
                using var accent = new SolidBrush(Primario);
                e.Graphics.FillRectangle(accent, 0, 0, 4, tarjeta.Height);
            };

            var lblModulo = new Label
            {
                Text = modulo.ToUpperInvariant(),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Primario,
                AutoSize = true,
                Location = new Point(16, 16),
                BackColor = Color.Transparent
            };

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Texto,
                AutoSize = true,
                Location = new Point(16, 38),
                BackColor = Color.Transparent
            };

            var lblDesc = new Label
            {
                Text = descripcion,
                Font = FuenteSubtitulo,
                ForeColor = TextoSecundario,
                Size = new Size(228, 52),
                Location = new Point(16, 66),
                BackColor = Color.Transparent
            };

            var lblIr = new Label
            {
                Text = "Abrir →",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Primario,
                AutoSize = true,
                Location = new Point(16, 124),
                BackColor = Color.Transparent
            };

            tarjeta.Controls.AddRange([lblModulo, lblTitulo, lblDesc, lblIr]);

            void ActivarHover(object? s, EventArgs e) => tarjeta.BackColor = PrimarioClaro;
            void DesactivarHover(object? s, EventArgs e) => tarjeta.BackColor = Tarjeta;
            void Click(object? s, EventArgs e) => alHacerClic();

            tarjeta.MouseEnter += ActivarHover;
            tarjeta.MouseLeave += DesactivarHover;
            tarjeta.Click += Click;
            foreach (Control c in tarjeta.Controls)
            {
                c.MouseEnter += ActivarHover;
                c.MouseLeave += DesactivarHover;
                c.Click += Click;
            }

            return tarjeta;
        }

        public static Button CrearBotonSidebar(string texto, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = "  " + texto,
                Dock = DockStyle.Top,
                Height = 44,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.White,
                BackColor = Primario,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand,
                Padding = new Padding(12, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = SidebarHover;
            btn.Click += onClick;
            return btn;
        }

        public static void ConfigurarSplitVertical(
            SplitContainer split,
            double ratioPanel1 = 0.58,
            int panel1Min = 260,
            int panel2Min = 260)
        {
            split.SplitterWidth = 4;

            void Ajustar()
            {
                if (split.Width <= 0 || split.IsDisposed)
                {
                    return;
                }

                var anchoMinimoTotal = panel1Min + panel2Min + split.SplitterWidth;
                if (split.Width < anchoMinimoTotal)
                {
                    var tercio = Math.Max(80, (split.Width - split.SplitterWidth) / 3);
                    split.Panel1MinSize = tercio;
                    split.Panel2MinSize = tercio;
                }
                else
                {
                    split.Panel1MinSize = panel1Min;
                    split.Panel2MinSize = panel2Min;
                }

                var maximo = split.Width - split.Panel2MinSize - split.SplitterWidth;
                var minimo = split.Panel1MinSize;
                if (maximo < minimo)
                {
                    return;
                }

                var deseado = (int)(split.Width * ratioPanel1);
                try
                {
                    split.SplitterDistance = Math.Clamp(deseado, minimo, maximo);
                }
                catch (InvalidOperationException)
                {
                    // El control aún no tiene tamaño definitivo; se reintenta en el próximo Resize.
                }
            }

            split.Resize += (_, _) => Ajustar();
            split.HandleCreated += (_, _) => Ajustar();
            if (split.IsHandleCreated)
            {
                Ajustar();
            }
        }

        public static void PintarFondoGradiente(Panel panel, PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Primario,
                PrimarioOscuro,
                LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, panel.ClientRectangle);
        }

        public static void AjustarBotonToolbar(Button boton, bool primario = false)
        {
            if (primario)
            {
                AplicarBotonPrimario(boton);
            }
            else
            {
                AplicarBotonSecundario(boton);
            }

            boton.AutoSize = true;
            boton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            boton.MinimumSize = new Size(104, 36);
            boton.Padding = new Padding(14, 6, 14, 6);
            boton.Margin = new Padding(0, 0, 8, 0);
        }

        public static TableLayoutPanel CrearFilaBusqueda(Control campo, params Button[] botones)
        {
            var tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1 + botones.Length,
                RowCount = 1,
                Margin = new Padding(0),
                AutoSize = false
            };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            foreach (var _ in botones)
            {
                tabla.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }

            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));

            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(0, 0, 8, 0);
            tabla.Controls.Add(campo, 0, 0);

            for (var i = 0; i < botones.Length; i++)
            {
                var boton = botones[i];
                boton.Margin = i < botones.Length - 1
                    ? new Padding(0, 0, 8, 0)
                    : new Padding(0);
                tabla.Controls.Add(boton, i + 1, 0);
            }

            return tabla;
        }

        public static Panel CrearPanelBusqueda(
            string etiqueta,
            Control campo,
            IReadOnlyList<Button> botones,
            params Control[] filasInferiores)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12, 10, 12, 10),
                BackColor = Tarjeta
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Margin = new Padding(0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var filaSuperior = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                Margin = new Padding(0, 0, 0, 8)
            };
            filaSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filaSuperior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filaSuperior.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lbl = new Label
            {
                Text = etiqueta,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 10, 12, 0),
                Font = FuenteNormal
            };

            var filaCampo = CrearFilaBusqueda(campo, botones.ToArray());
            filaCampo.Dock = DockStyle.Fill;
            filaCampo.Height = 38;

            filaSuperior.Controls.Add(lbl, 0, 0);
            filaSuperior.Controls.Add(filaCampo, 1, 0);
            AgregarFilaAuto(layout, filaSuperior);

            foreach (var control in filasInferiores)
            {
                control.Dock = DockStyle.Top;
                control.Margin = new Padding(0, 0, 0, 4);
                AgregarFilaAuto(layout, control);
            }

            panel.Controls.Add(layout);
            panel.Paint += (_, e) =>
            {
                using var pen = new Pen(Borde);
                e.Graphics.DrawLine(pen, 0, panel.Height - 1, panel.Width, panel.Height - 1);
            };

            return panel;
        }

        public static FlowLayoutPanel CrearBarraBotones(params Button[] botones)
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(4, 12, 4, 4),
                Margin = new Padding(0)
            };

            foreach (var boton in botones)
            {
                boton.MinimumSize = new Size(96, 38);
                boton.Margin = new Padding(0, 0, 8, 8);
                panel.Controls.Add(boton);
            }

            return panel;
        }

        public static TableLayoutPanel CrearCampoVertical(string etiqueta, Control campo, Control? extra = null)
        {
            var contenedor = new TableLayoutPanel
            {
                ColumnCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 10)
            };
            contenedor.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            contenedor.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            contenedor.Controls.Add(new Label
            {
                Text = etiqueta,
                ForeColor = Texto,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 6)
            }, 0, 0);

            if (extra is null)
            {
                campo.Dock = DockStyle.Top;
                campo.Margin = new Padding(0);
                contenedor.Controls.Add(campo, 0, 1);
                return contenedor;
            }

            var fila = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0)
            };
            fila.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            fila.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92F));
            fila.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(0, 0, 8, 0);
            extra.Dock = DockStyle.Fill;
            extra.Margin = new Padding(0);
            fila.Controls.Add(campo, 0, 0);
            fila.Controls.Add(extra, 1, 0);
            contenedor.Controls.Add(fila, 0, 1);
            return contenedor;
        }

        private static void AgregarFilaAuto(TableLayoutPanel tabla, Control control)
        {
            var fila = tabla.RowCount;
            tabla.RowCount++;
            tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            control.Dock = DockStyle.Top;
            tabla.Controls.Add(control, 0, fila);
        }
    }
}
