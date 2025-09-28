using microUsuarios.API.Model;
using microUsuarios.API.Utils;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using microUsuarios.API.Model.Request;
using microUsuarios.API.Dao;
using microUsuarios.API.Model.Response;

namespace microUsuarios.API.Logic
{
    public class BLEmpleado
    {
        public static GeneralResponse IniciarSesionEmpleado(IniciarSesionRequest request)
        {
            var res = DAEmpleado.ObtenerEmpleado(-1, request);

            if (res.status == Variables.Response.OK)
            {
                var empleado = (EmpleadoResponse)res.data;
                string token = JWTHelper.GenerarToken(empleado.Id, empleado.Correo, empleado.Rol);
                res.message = "Inicio de sesion exitoso";
                res.data = token;
            }
            else
            {
                return res;
            }
            return res;
        }
        public static GeneralResponse CrearEmpleado(AgregarUsuarioRequest request)
        {
            //Validar que no exista un empleado con nuemro de documento o correo repetido 
            var validar = DAEmpleado.ValidarEmpleado(request);
            if (validar.status == Variables.Response.ERROR && (bool)validar.data == true)
            {
                return validar; 
            }
            var res = DAEmpleado.CrearEmpleado(request);
            return res;
        }
        public static GeneralResponse ObtenerEmpleado(int idEmpleado)
        {
            var res = DAEmpleado.ObtenerEmpleado(idEmpleado);
            return res;
        }
        public static GeneralResponse ObtenerEmpleados()
        {
            var res = DAEmpleado.ObtenerEmpleados();
            return res;
        }

        public static GeneralResponse EliminarEmpleado(int idEmpleado, int idAdmin)
        {
            var res = DAEmpleado.EliminarEmpleado(idEmpleado, idAdmin);
            return res;
        }

        public static GeneralResponse ActualizarEmpleado(AgregarUsuarioRequest request, int idUsuario)
        {
            //Validar que no exista un empleado con nuemro de documento o correo repetido 
            var validar = DAEmpleado.ValidarEmpleado(request);
            if (validar.status == Variables.Response.OK && (bool)validar.data == false)
            {
                return validar;
            }
            var res = DAEmpleado.ActualizarEmpleado(request, idUsuario);
            return res;
        }
    }
}
