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
                if (string.IsNullOrEmpty(gateId))
                {
                    return BadRequest(new { error = "Gate ID is required" });
                }

                if (string.IsNullOrEmpty(request?.Reason))
                {
                    return BadRequest(new { error = "Capture reason is required" });
                }

                if (!new[] { "ENTRY", "EXIT", "MANUAL" }.Contains(request.Reason.ToUpper()))
                {
                    return BadRequest(new { error = "Invalid reason. Must be ENTRY, EXIT, or MANUAL" });
                }

                // Check if camera is operational
                if (!await _cameraService.IsOperationalAsync(gateId))
                {
                    return StatusCode(503, new { error = "Camera is not operational" });
                }

                // Capture image using camera service
                var imageData = await _cameraService.CaptureImageAsync(gateId, request.Reason);
                if (imageData == null)
                {
                    return StatusCode(503, new { error = "Failed to capture image" });
                }

                // Convert byte[] to Base64 string for storing or transmitting
                string imagePath = Convert.ToBase64String(imageData);

                var response = new CameraCaptureResponse
                {
                    Success = true,
                    Message = "Image captured successfully",
                    GateId = gateId,
                    ImageInfo = new ImageInfo
                    {
                        ExpectedPath = imagePath,
                        CaptureTime = DateTime.UtcNow,
                        Reason = request.Reason
                    }
                };

                // Notify clients through SignalR
                await _hubContext.Clients.All.SendAsync("CameraCaptureTriggered", response);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Invalid operation when triggering camera for gate {gateId}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error triggering camera for gate {gateId}");
                return StatusCode(500, new { error = "An unexpected error occurred while capturing the image" });
            }
        }

        [HttpGet("{gateId}/camera/status")]
        [Authorize]
        public async Task<ActionResult<CameraStatusResponse>> GetCameraStatus(string gateId)
        {
            try
            {
                if (string.IsNullOrEmpty(gateId))
                {
                    return BadRequest(new { error = "Gate ID is required" });
                }

                var camera = await _cameraService.GetCameraByGateIdAsync(gateId);
                if (camera == null)
                {
                    return NotFound(new { error = $"No camera found for gate {gateId}" });
                }

                var isOperational = await _cameraService.IsOperationalAsync(gateId);

                return Ok(new CameraStatusResponse
                {
                    GateId = gateId,
                    IsOperational = isOperational,
                    LastChecked = DateTime.UtcNow,
                    CameraInfo = new CameraInfo
                    {
                        Name = camera.Name,
                        Model = camera.Model,
                        Resolution = camera.Resolution,
                        Status = camera.Status,
                        LastError = camera.LastError
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking camera status for gate {gateId}");
                return StatusCode(500, new { error = "An unexpected error occurred while checking camera status" });
            }
        }
    }

    public class CameraCaptureRequest
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class CameraCaptureResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string GateId { get; set; } = string.Empty;
        public ImageInfo ImageInfo { get; set; } = new();
    }

    public class ImageInfo
    {
        public string ExpectedPath { get; set; } = string.Empty;
        public DateTime CaptureTime { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class CameraStatusResponse
    {
        public string GateId { get; set; } = string.Empty;
        public bool IsOperational { get; set; }
        public DateTime LastChecked { get; set; }
        public CameraInfo CameraInfo { get; set; } = new();
    }

    public class CameraInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? LastError { get; set; }
    }
} 