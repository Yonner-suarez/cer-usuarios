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
    public class ClienteController : ControllerBase
    {
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
        [HttpPost("")]
        public IActionResult Cliente(AgregarUsuarioRequest request)
        {
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
        [HttpDelete("/{idCliente}")]
        public IActionResult EliminarCuentaCliente([Required] int idCliente)
        {
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

    }
}
