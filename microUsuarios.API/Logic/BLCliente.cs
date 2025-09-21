using microUsuarios.API.Dao;
using microUsuarios.API.Model.Request;
using microUsuarios.API.Model;
using microUsuarios.API.Utils;
using microUsuarios.API.Model.Response;
using microUsuarios.API.Utils.ApiExterna;

namespace microUsuarios.API.Logic
{
    public class BLCliente
    {
        public static GeneralResponse IniciarSesionCliente(IniciarSesionRequest request)
        {
            var res = DACliente.ObtenerCliente(-1, request);

            if(res.status == Variables.Response.OK)
            {
                var cliente = (ClienteResponse)res.data;
                string token = JWTHelper.GenerarToken(cliente.Id, cliente.Correo, cliente.Rol);
                res.message = "Inicio de sesion exitoso";
                res.data = token;
            }
            else
            {
                return res;
            }
            return res;
        }
        public static GeneralResponse CrearCliente(AgregarUsuarioRequest request)
        {
            //Validar que no exista un cliente con correo o numero de doc en la DB
            var validar = DACliente.ValidarCliente(request);
            if (validar.status == Variables.Response.ERROR && validar.data is true)
            {
                return validar;
            }
            var res = DACliente.CrearCliente(request);
            return res;
        }
        public static GeneralResponse EliminarCuentaCliente(int idCliente)
        {
            //Validar que exista un cliente con correo o numero de doc en la DB para eliminar
            var cliente = DACliente.ObtenerCliente(idCliente);
            if(cliente.status != Variables.Response.OK)
            {
                return cliente;
            }
            var res = DACliente.EliminarCuentaCliente(idCliente);
            return res;
        }

        public static async Task<GeneralResponse> ObtenerCliente(int idCliente)
        {
            //Validar que no exista un cliente con correo o numero de doc en la DB
            var cliente = DACliente.ObtenerCliente(idCliente);
            if (cliente.status != Variables.Response.OK)
            {
                return cliente;
            }
            //Obtener Carrito al api de pedidos
            var cliente_aux = (ClienteResponse)cliente.data;
            string token = JWTHelper.GenerarToken(cliente_aux.Id, cliente_aux.Correo, cliente_aux.Rol);
            var carrito_cliente = await CarritoCliente.ObtenerCarritoAsync(token);
            if (carrito_cliente?.Items != null)
            {
                cliente_aux.Items = carrito_cliente.Items;
            }

            // Retornar en GeneralResponse
            return new GeneralResponse
            {
                status = Variables.Response.OK,
                message = "Cliente obtenido correctamente",
                data = cliente_aux
            };
        }
    }
}
