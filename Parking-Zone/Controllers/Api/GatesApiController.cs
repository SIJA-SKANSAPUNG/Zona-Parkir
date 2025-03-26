using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Parking_Zone.Hubs;
using Parking_Zone.Models;
using Parking_Zone.Services;
using System;
using System.Threading.Tasks;

namespace Parking_Zone.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class GatesApiController : ControllerBase
    {
        private readonly ILogger<GatesApiController> _logger;
        private readonly IHubContext<GateHub> _hubContext;
        private readonly ICameraService _cameraService;

        public GatesApiController(
            ILogger<GatesApiController> logger,
            IHubContext<GateHub> hubContext,
            ICameraService cameraService)
        {
            _logger = logger;
            _hubContext = hubContext;
            _cameraService = cameraService;
        }

        [HttpPost("{gateId}/camera/capture")]
        [Authorize]
        public async Task<ActionResult<CameraCaptureResponse>> TriggerCamera(string gateId, [FromBody] CameraCaptureRequest request)
        {
            try
            {
                if (!new[] { "entry", "exit" }.Contains(gateId.ToLower()))
                {
                    return NotFound(new { error = "Gate not found" });
                }

                if (!new[] { "ENTRY", "EXIT", "MANUAL" }.Contains(request.Reason))
                {
                    return BadRequest(new { error = "Invalid reason" });
                }

                // Check if camera is operational
                if (!await _cameraService.IsOperationalAsync(gateId))
                {
                    return StatusCode(503, new { error = "Camera is not operational" });
                }

                // Capture image using camera service
                var imagePath = await _cameraService.CaptureImageAsync(gateId, request.Reason);

                var response = new CameraCaptureResponse
                {
                    Success = true,
                    Message = "Image capture triggered",
                    GateId = gateId,
                    ImageInfo = new ImageInfo
                    {
                        ExpectedPath = imagePath
                    }
                };

                // Notify clients through SignalR
                await _hubContext.Clients.All.SendAsync("CameraCaptureTriggered", response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error triggering camera for gate {gateId}");
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("{gateId}/camera/status")]
        [Authorize]
        public async Task<ActionResult<CameraStatusResponse>> GetCameraStatus(string gateId)
        {
            try
            {
                if (!new[] { "entry", "exit" }.Contains(gateId.ToLower()))
                {
                    return NotFound(new { error = "Gate not found" });
                }

                var isOperational = await _cameraService.IsOperationalAsync(gateId);

                return Ok(new CameraStatusResponse
                {
                    GateId = gateId,
                    IsOperational = isOperational,
                    LastChecked = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking camera status for gate {gateId}");
                return StatusCode(500, new { error = $"Internal server error: {ex.Message}" });
            }
        }
    }

    public class CameraCaptureRequest
    {
        public string Reason { get; set; }
    }

    public class CameraCaptureResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string GateId { get; set; }
        public ImageInfo ImageInfo { get; set; }
    }

    public class ImageInfo
    {
        public string ExpectedPath { get; set; }
    }

    public class CameraStatusResponse
    {
        public string GateId { get; set; }
        public bool IsOperational { get; set; }
        public DateTime LastChecked { get; set; }
    }
} 