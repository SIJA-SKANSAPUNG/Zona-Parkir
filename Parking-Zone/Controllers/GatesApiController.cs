using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Parking_Zone.Models;
using Parking_Zone.Services;
using System;
using System.Threading.Tasks;

namespace Parking_Zone.Controllers
{
    [Route("api/gates")]
    [ApiController]
    public class GatesApiController : ControllerBase
    {
        private readonly IParkingGateService _gateService;
        private readonly ILogger<GatesApiController> _logger;

        public GatesApiController(IParkingGateService gateService, ILogger<GatesApiController> logger)
        {
            _gateService = gateService ?? throw new ArgumentNullException(nameof(gateService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGates()
        {
            try
            {
                var entryGates = await _gateService.GetAllEntryGatesAsync();
                var exitGates = await _gateService.GetAllExitGatesAsync();
                var allGates = entryGates.Concat(exitGates);
                
                return Ok(allGates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all gates");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGate(Guid id)
        {
            try
            {
                var gate = await _gateService.GetGateByIdAsync(id);
                if (gate == null)
                    return NotFound();

                return Ok(gate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving gate {GateId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/command")]
        public async Task<IActionResult> SendCommand(Guid id, [FromBody] GateCommandDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                ParkingGate gate;
                if (dto.Command.ToLower() == "open")
                    gate = await _gateService.OpenGateAsync(id);
                else if (dto.Command.ToLower() == "close")
                    gate = await _gateService.CloseGateAsync(id);
                else
                    return BadRequest("Invalid command. Use 'open' or 'close'.");

                return Ok(gate);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending command to gate {GateId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetGateStatus(Guid id)
        {
            try
            {
                var gate = await _gateService.GetGateByIdAsync(id);
                if (gate == null)
                    return NotFound();

                return Ok(new { gate.IsOpen, gate.IsOnline, gate.LastActivity });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving status for gate {GateId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class GateCommandDto
    {
        public string Command { get; set; }
    }
}
