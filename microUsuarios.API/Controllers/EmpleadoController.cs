using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using microUsuarios.API.Logic;
using microUsuarios.API.Model.Request;
using microUsuarios.API.Utils;
using System.ComponentModel.DataAnnotations;

namespace microUsuarios.API.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        [HttpPost("[action]")]
        public IActionResult Login(IniciarSesionRequest request)
        {
            var res = BLEmpleado.IniciarSesionEmpleado(request);
            if (res.status == Variables.Response.OK)
            {
                return Ok(res);
            }
            else
            {
                return StatusCode(res.status, res);
            }
        }
        [HttpPost("")] // Admin o Logística
        public IActionResult Empleado(AgregarUsuarioRequest request)
        {
            var res = BLEmpleado.CrearEmpleado(request);
            if (res.status == Variables.Response.OK)
            {
                return Ok(res);
            }
            else
            {
                return StatusCode(res.status, res);
            }
        }

        [HttpGet("/{idEmpleado}")] // Admin o Logística
        public IActionResult ObtenerEmpleado([Required] int idEmpleado)
        {
            var res = BLEmpleado.ObtenerEmpleado(idEmpleado);
            if (res.status == Variables.Response.OK)
            {
                return Ok(res);
            }
            else
            {
                return StatusCode(res.status, res);
            }
        }
        [HttpGet("Empleados")] // Admin y Logística
        public IActionResult ObtenerEmpleados()
        {
            var res = BLEmpleado.ObtenerEmpleados();
            if (res.status == Variables.Response.OK)
            {
                return Ok(res);
            }
            else
            {
                return StatusCode(res.status, res);
            }
        }
        [HttpDelete("/{idEmpleado}/{idAdmin}")]
        public IActionResult EliminarEmpleado([Required] int idEmpleado, int idAdmin)
        {
            var res = BLEmpleado.EliminarEmpleado(idEmpleado, idAdmin);
            if (res.status == Variables.Response.OK)
            {
                return Ok(res);
            }
            else
            {
                return StatusCode(res.status, res);
            }
        }
    }
}
