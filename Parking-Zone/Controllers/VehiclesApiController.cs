using Microsoft.AspNetCore.Mvc;
using Parking_Zone.Models;
using Parking_Zone.Services;
using System;
using System.Threading.Tasks;

namespace Parking_Zone.Controllers
{
    [Route("api/vehicles")]
    [ApiController]
    public class VehiclesApiController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehiclesApiController> _logger;

        public VehiclesApiController(IVehicleService vehicleService, ILogger<VehiclesApiController> logger)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/vehicles
        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            try
            {
                var vehicles = await _vehicleService.GetAllVehicles();
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all vehicles");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/vehicles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle(Guid id)
        {
            try
            {
                var vehicle = await _vehicleService.GetVehicleById(id);
                if (vehicle == null)
                    return NotFound();

                return Ok(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vehicle {VehicleId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/vehicles/entry
        [HttpPost("entry")]
        public async Task<IActionResult> RecordEntry([FromBody] VehicleEntryDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var vehicle = await _vehicleService.RecordEntry(dto.PlateNumber, dto.VehicleType, dto.PhotoEntry, dto.ParkingZoneId);
                return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicle);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording vehicle entry for {PlateNumber}", dto.PlateNumber);
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/vehicles/{id}/exit
        [HttpPut("{id}/exit")]
        public async Task<IActionResult> RecordExit(Guid id, [FromBody] VehicleExitDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var vehicle = await _vehicleService.RecordExit(id, dto.PhotoExit);
                return Ok(vehicle);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording vehicle exit for {VehicleId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class VehicleEntryDto
    {
        public string PlateNumber { get; set; }
        public string VehicleType { get; set; }
        public byte[] PhotoEntry { get; set; }
        public Guid ParkingZoneId { get; set; }
    }

    public class VehicleExitDto
    {
        public byte[] PhotoExit { get; set; }
    }
}
