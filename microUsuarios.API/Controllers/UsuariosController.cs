using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace microUsuarios.API.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsuariosController: ControllerBase
    {
        [HttpPost]
        [Route("[action]")]
        public ActionResult usuario(int idEmpleado)
        {
            //GeneralResponse res = BLActivacionContrato.ListarEstados(idEmpleado);
            return Ok();
        }
    }
}
