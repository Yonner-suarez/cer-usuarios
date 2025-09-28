namespace microUsuarios.API.Model.Request
{
    public class AgregarUsuarioRequest
    {
        public int IdAdmin { get; set; } = 0;
        public int NroDocumento { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contrasenia { get; set; }
        /// <summary>
        /// 'Cliente','Administrador','Logistica'
        /// </summary>
        public string Cargo { get; set; } = "Cliente";

        /// <summary>
        /// 'Natural', 'Juridica'
        /// </summary>
        public string TipoPersona { get; set; } = "";
        /// <summary>
        /// Codigo podstal del cliente
        /// </summary>
        public string CodigoPostal { get; set; } = "";
        /// <summary>
        /// direccion del cliente
        /// </summary>
        public string Direccion { get; set; } = "";
        /// <summary>
        /// Nro de telefono
        /// </summary>
        public string Telefono { get; set; } = "";
    }
}
