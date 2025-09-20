namespace microUsuarios.API.Model.Response
{
    public class EmpleadoResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contrasenia { get; set; }
        public string Documento { get; set; }
        public string Rol { get; set; } // Cliente, Administrador, Logistica
        public string TipoPersona { get; set; } // Natural, Jurídica
        public DateTime FechaCreacion { get; set; }
        public int CreadoPor { get; set; }
    }
}
