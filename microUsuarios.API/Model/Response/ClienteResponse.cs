namespace microUsuarios.API.Model.Response
{
    public class ClienteResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Documento { get; set; }
        public string TipoPersona { get; set; }
        public string CodigoPostal { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int CreadoPor { get; set; }
        public string Rol { get; set; }
        public List<CarritoItem> Items { get; set; } = new();
    }

}
