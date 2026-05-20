using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using Server.Application.Interfaces;
using Server.Domain;
using Server.Application.DTOs;
using Server.Infrastructure.BackgroundServices;

namespace Server.Infrastructure.Controllers
{
    [ApiController]
    [Route("api/server")]
    public class ServerController : ControllerBase
    {

        private readonly IServerService _service;
        private readonly PlcDataCache _cache;
        private readonly PlcConnection _plcConnection;

        public ServerController(IServerService service,PlcDataCache cache,PlcConnection plcConnection)
        {
            _service = service;
            _cache = cache;
            _plcConnection = plcConnection;
        }

        //-------------------PLC GET/UPDATE CONNECTION SETTINGS-----------------------------
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

        //-------------------PLC GET/POST TO L1-----------------------------
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetEafDataFromPlc()
        {
            try
            {
                var data = _cache.Get();
                if (data == null)
                    return NoContent();
                return Ok(data);
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

        [HttpPost]
        [Route("[action]")]
        public IActionResult WriteBoolToPlc([FromQuery] string variable, [FromQuery] bool state)
        {
            try
            {
                _plcConnection.WriteBool(variable, state);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(503, ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult LoadScrap()
        {
            try
            {
                _service.LoadScrap();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Tap()
        {
            try
            {
                _service.Tap();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Reset()
        {
            try
            {
                _service.Reset();
                return Ok();
            }

        [HttpPost]
        [Route("[action]")]
        public IActionResult WriteRealToPlc([FromQuery] string variable, [FromQuery] float value)
        {
            try
            {
                _plcConnection.WriteReal(variable, (float)value);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(503, ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

    }
}
 


       