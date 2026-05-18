using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using Server.Application.Interfaces;
using Server.Domain;
    using Server.Application.DTOs;

namespace Server.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/server")]
    public class ServerController : ControllerBase
    {

        private readonly IServerService _service;

        public ServerController(IServerService service)
        {
            _service = service;
        }


        [HttpGet]
        [Route("[action]")]
        public IActionResult GetPlc()
        {
            try
            {
                PlcDto plc = _service.GetPlc();
                return Ok(plc);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

    }
}
 


       