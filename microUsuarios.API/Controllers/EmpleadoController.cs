using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using microUsuarios.API.Logic;
using microUsuarios.API.Model.Request;
using microUsuarios.API.Utils;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Principal;
using static microUsuarios.API.Utils.Variables;

namespace microUsuarios.API.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        [AllowAnonymous]
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
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null) return StatusCode(Variables.Response.Inautorizado, null);


            var claims = identity.Claims;
            var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if(role != "Administrador") return StatusCode(Variables.Response.BadRequest, "Solo los Administradores puede crear un Empleado");

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
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null) return StatusCode(Variables.Response.Inautorizado, null);


            var claims = identity.Claims;
            var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != "Administrador") return StatusCode(Variables.Response.BadRequest, "Solo los Administradores puede crear un Empleado");

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
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null) return StatusCode(Variables.Response.Inautorizado, null);

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
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null) return StatusCode(Variables.Response.Inautorizado, null);

            var claims = identity.Claims;
            var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != "Administrador") return StatusCode(Variables.Response.BadRequest, "Solo los Administradores puede crear un Empleado");

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


//var claims = identity.Claims;

//// Buscar por tipo estándar
//var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

//// Buscar por rol
//var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

//// Buscar por clave personalizada (ej: idCliente)
//var idCliente = claims.FirstOrDefault(c => c.Type == "idUser")?.Value;