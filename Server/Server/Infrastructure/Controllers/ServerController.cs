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

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpdatePlc([FromBody] PlcDto plcdto)
        {
            try
            {
                _service.UpdatePlc(plcdto);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult SetCurrent([FromBody] float current)
        {
            try
            {
                _service.SetCurrent(current);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult SetAngle([FromBody] float angle)
        {
            try
            {
                _service.SetAngle(angle);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetEnergyConsumed()
        {
            try
            {
                float energy_consumed = _service.GetEnergyConsumed();
                return Ok(energy_consumed);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetEAF()
        {
            try
            {
                EAFDto eaf = _service.GetEAF();
                return Ok(eaf);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult PostEAF([FromBody] EAFDto eaf)
        {
            try
            {
                _service.PostEAF(eaf);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
 


       