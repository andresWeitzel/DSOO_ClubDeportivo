namespace TP_ClubDeportivo.Data
{
    public sealed record DatosInstalacionMySql(
        string Servidor,
        string Puerto,
        string Usuario,
        string Clave,
        string BaseDatos)
    {
        public string ResumenTexto =>
            $"Servidor: {Servidor}\n" +
            $"Puerto: {Puerto}\n" +
            $"Usuario: {Usuario}\n" +
            $"Clave: {(string.IsNullOrEmpty(Clave) ? "(vacía)" : Clave)}\n" +
            $"Base de datos: {BaseDatos}";
    }
}
