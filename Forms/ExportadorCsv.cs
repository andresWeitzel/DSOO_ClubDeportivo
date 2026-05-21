using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TP_ClubDeportivo.Forms
{
    internal static class ExportadorCsv
    {
        public static bool ExportarGrilla(DataGridView grilla, string tituloReporte)
        {
            if (grilla.Rows.Count == 0)
            {
                MessageBox.Show(
                    "No hay datos para exportar. Genere el reporte primero.",
                    "Exportar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }

            var nombreArchivo = $"{SanitizarNombreArchivo(tituloReporte)}_{DateTime.Now:yyyyMMdd_HHmm}.csv";

            using var dialogo = new SaveFileDialog
            {
                Filter = "Archivo CSV (*.csv)|*.csv",
                FileName = nombreArchivo,
                Title = "Exportar reporte"
            };

            if (dialogo.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            var columnas = grilla.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine(string.Join(";", columnas.Select(c => Escapar(c.HeaderText))));

            foreach (DataGridViewRow row in grilla.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                var valores = columnas.Select(c =>
                {
                    var valor = row.Cells[c.Index].FormattedValue?.ToString() ?? string.Empty;
                    return Escapar(valor);
                });

                sb.AppendLine(string.Join(";", valores));
            }

            File.WriteAllText(dialogo.FileName, sb.ToString(), Encoding.UTF8);

            MessageBox.Show(
                $"Reporte exportado correctamente.\n\n{dialogo.FileName}",
                "Exportar",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return true;
        }

        private static string Escapar(string valor)
        {
            if (valor.Contains(';') || valor.Contains('"') || valor.Contains('\n'))
            {
                return $"\"{valor.Replace("\"", "\"\"")}\"";
            }

            return valor;
        }

        private static string SanitizarNombreArchivo(string nombre)
        {
            var invalidos = Path.GetInvalidFileNameChars();
            var limpio = new string(nombre.Select(c => invalidos.Contains(c) ? '_' : c).ToArray());
            return string.IsNullOrWhiteSpace(limpio) ? "reporte" : limpio.Replace(' ', '_');
        }
    }
}
