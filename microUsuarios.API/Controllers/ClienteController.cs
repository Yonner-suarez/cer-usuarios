using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using microUsuarios.API.Logic;
using microUsuarios.API.Model.Request;
using microUsuarios.API.Utils;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace microUsuarios.API.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("[action]")]
        public IActionResult Login(IniciarSesionRequest request)
        {
            var res = BLCliente.IniciarSesionCliente(request);
            if (res.status == Variables.Response.OK)
            {
                return Ok(res);
            }
            else
            {
                return StatusCode(res.status, res);
            }
        }
        [AllowAnonymous]
        [HttpPost("")]
        public IActionResult Cliente(AgregarUsuarioRequest request)
        {
            //No se necesita es tener aut para este EP porque el cliente aun no existe
            var res = BLCliente.CrearCliente(request);
            if (res.status == Variables.Response.OK)
            {
                return Ok(res);
            }
            else
            {
                return StatusCode(res.status, res);
            }
        }
        [HttpDelete("")]
        public IActionResult EliminarCuentaCliente()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null) return StatusCode(Variables.Response.Inautorizado, null);


            var claims = identity.Claims;
            var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != "Cliente") return StatusCode(Variables.Response.BadRequest, "Solo el cliente puede eliminar su cuenta");
            var idCliente = int.Parse(claims.FirstOrDefault(c => c.Type == "idUser")?.Value);

            var res = BLCliente.EliminarCuentaCliente(idCliente);
            if (res.status == Variables.Response.OK)
            {
                return Ok(res);
            }
            else
            {
                return StatusCode(res.status, res);
            }
        }

        [HttpGet("")]
        public async Task<IActionResult> ObtenerCliente()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null) return StatusCode(Variables.Response.Inautorizado, null);


            var claims = identity.Claims;
            var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != "Cliente") return StatusCode(Variables.Response.BadRequest, "Solo el cliente puede acceder su cuenta");
            var idCliente = int.Parse(claims.FirstOrDefault(c => c.Type == "idUser")?.Value);

            var res = await BLCliente.ObtenerCliente(idCliente );
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
