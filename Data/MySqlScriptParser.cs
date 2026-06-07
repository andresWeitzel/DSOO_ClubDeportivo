using System.Text;

namespace TP_ClubDeportivo.Data
{
    internal static class MySqlScriptParser
    {
        public static IEnumerable<string> Dividir(string script)
        {
            var delimitador = ";";
            var actual = new StringBuilder();

            foreach (var lineaRaw in script.Split('\n'))
            {
                var linea = lineaRaw.TrimEnd('\r');
                var recortada = linea.Trim();

                if (recortada.StartsWith("DELIMITER ", StringComparison.OrdinalIgnoreCase))
                {
                    var partes = recortada.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (partes.Length >= 2)
                    {
                        delimitador = partes[1];
                    }

                    continue;
                }

                if (EsSoloComentario(recortada))
                {
                    continue;
                }

                actual.AppendLine(linea);

                if (!TerminaConDelimitador(linea, delimitador))
                {
                    continue;
                }

                var sentencia = actual.ToString().Trim();
                actual.Clear();

                if (EsSentenciaEjecutable(sentencia))
                {
                    yield return sentencia;
                }
            }

            var restante = actual.ToString().Trim();
            if (EsSentenciaEjecutable(restante))
            {
                yield return restante;
            }
        }

        private static bool TerminaConDelimitador(string linea, string delimitador)
        {
            var recortada = linea.TrimEnd();
            return recortada.EndsWith(delimitador, StringComparison.Ordinal);
        }

        private static bool EsSoloComentario(string linea)
        {
            return linea.StartsWith("--", StringComparison.Ordinal)
                || linea.StartsWith('#')
                || linea.Length == 0;
        }

        private static bool EsSentenciaEjecutable(string sentencia)
        {
            if (string.IsNullOrWhiteSpace(sentencia))
            {
                return false;
            }

            foreach (var linea in sentencia.Split('\n'))
            {
                var recortada = linea.Trim();
                if (recortada.Length == 0 || EsSoloComentario(recortada))
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}
