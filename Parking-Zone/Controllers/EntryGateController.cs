using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Extensions;
using Parking_Zone.Hardware;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Parking_Zone.Hubs;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Parking_Zone.Enums;

// Resolve VehicleType ambiguity
using VehicleType = Parking_Zone.Enums.VehicleType;

namespace Parking_Zone.Controllers
{
    [Authorize(Roles = "Admin,Operator")]
    public class EntryGateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EntryGateController> _logger;
        private readonly ICameraService _cameraService;
        private readonly StorageService _storageService;
        private readonly ITicketService _ticketService;
        private readonly IPrinterService _printerService;
        private readonly IHubContext<ParkingHub> _parkingHubContext;

        public EntryGateController(
            ApplicationDbContext context,
            ILogger<EntryGateController> logger,
            ICameraService cameraService,
            StorageService storageService,
            ITicketService ticketService,
            IPrinterService printerService,
            IHubContext<ParkingHub> parkingHubContext)
        {
            _context = context;
            _logger = logger;
            _cameraService = cameraService;
            _storageService = storageService;
            _ticketService = ticketService;
            _printerService = printerService;
            _parkingHubContext = parkingHubContext;
        }

        private VehicleEntryModel ConvertToViewModel(Models.VehicleEntry entry)
        {
            return new VehicleEntryModel
            {
                LicensePlate = entry.LicensePlate,
                VehicleType = Enum.Parse<VehicleType>(entry.VehicleType),
                EntryTime = entry.EntryTime,
                EntryOperator = entry.EntryOperator,
                PhotoEntry = entry.PhotoEntry,
                TicketBarcode = entry.TicketNumber,
                ParkingSpaceId = entry.ParkingSpaceId,
                OperatorId = entry.OperatorId,
                Notes = entry.Notes
            };
        }

        private VehicleEntryViewModel MapToVehicleEntryViewModel(VehicleEntryRequestViewModel request, Guid operatorId)
        {
            // Convert string to VehicleType
            var vehicleType = Enum.TryParse<VehicleType>(request.VehicleType.ToString(), out var parsedType) 
                ? parsedType 
                : VehicleType.Car;

            return new VehicleEntryViewModel
            {
                LicensePlate = request.PlateNumber,
                VehicleType = vehicleType,
                EntryTime = request.EntryTime,
                EntryOperator = request.EntryOperator,
                PhotoEntry = request.PhotoEntry ?? Array.Empty<byte>(),
                TicketBarcode = request.TicketBarcode,
                ParkingSpaceId = request.ParkingSpaceId,
                OperatorId = operatorId,
                Notes = request.Notes
            };
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new ViewModels.GatesViewModel
            {
                EntryGates = await _context.EntryGates
                    .Select(g => new ViewModels.GateOperationalViewModel
                    {
                        GateId = g.Id.ToString(),
                        OperatorName = g.Name,
                        Status = g.Status,
                        IsCameraActive = g.IsActive,
                        LastSync = g.LastActivity ?? DateTime.Now,
                        RecentEntries = g.Transactions
                            .Where(t => t.EntryTime.Date == DateTime.Today)
                            .Select(t => ConvertToViewModel(t))
                            .ToList()
                    })
                    .ToListAsync(),

                ExitGates = await _context.ExitGates
                    .Select(g => new ViewModels.GateExitOperationalViewModel
                    {
                        GateId = g.Id.ToString(),
                        OperatorName = g.Name,
                        Status = g.Status,
                        IsCameraActive = g.IsActive,
                        LastSync = g.LastActivity ?? DateTime.Now,
                        RecentExits = g.Transactions
                            .Where(t => t.ExitTime.HasValue && t.ExitTime.Value.Date == DateTime.Today)
                            .Select(t => new Models.VehicleExit
                            {
                                LicensePlate = t.LicensePlate,
                                VehicleType = t.VehicleType,
                                EntryTime = t.EntryTime,
                                ExitTime = t.ExitTime.Value,
                                Fee = t.ParkingFee ?? 0,
                                TicketNumber = t.TicketNumber,
                                GateId = g.Id.ToString(),
                                OperatorId = t.OperatorId.ToString()
                            })
                            .ToList()
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEntryGate(Models.EntryGate gate)
        {
            if (ModelState.IsValid)
            {
                gate.Id = Guid.NewGuid();
                gate.IsActive = true;
                gate.LastActivity = DateTime.Now;
                
                _context.EntryGates.Add(gate);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"New entry gate added: {gate.Name}");
                
                return RedirectToAction(nameof(Index));
            }
            
            return View(gate);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEntryGate(string id, bool isActive)
        {
            var gate = await _context.EntryGates.FindAsync(id);
            if (gate == null)
            {
                return NotFound();
            }
            
            gate.IsActive = isActive;
            gate.LastActivity = DateTime.Now;
            
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Entry gate {id} status updated: Active = {isActive}");
            
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetGateStatus(string id)
        {
            var gate = await _context.EntryGates.FindAsync(id);
            if (gate == null)
            {
                return NotFound();
            }
            
            return Json(new
            {
                id = gate.Id,
                name = gate.Name,
                isOnline = gate.IsOnline,
                isOpen = gate.IsOpen,
                lastActivity = gate.LastActivity
            });
        }

        [HttpPost("capture")]
        public async Task<IActionResult> CaptureEntry([FromBody] ViewModels.EntryRequestViewModel request)
        {
            try 
            {
                var fileName = $"{Guid.NewGuid()}_entry.jpg";
                byte[] imageBytes = request.PhotoEntry ?? Array.Empty<byte>();
                string imagePath = await _storageService.SaveImage(fileName, imageBytes);
                
                var vehicleEntry = new Models.VehicleEntry
                {
                    VehicleNumber = request.VehicleNumber,
                    LicensePlate = request.PlateNumber,
                    VehicleType = request.VehicleType,
                    EntryTime = request.EntryTime,
                    EntryOperator = request.EntryOperator,
                    PhotoEntry = imageBytes,
                    TicketBarcode = request.TicketBarcode
                };

                _context.VehicleEntries.Add(vehicleEntry);
                await _context.SaveChangesAsync();

                var ticket = await _ticketService.GenerateTicketAsync(
                    request.VehicleNumber, 
                    request.VehicleType, 
                    imagePath, 
                    parkingGateId: request.GateId, 
                    operatorId: User.GetOperatorGuid()
                );

                return Ok(new { 
                    ticketNumber = ticket.TicketNumber,
                    imagePath = imagePath,
                    vehicleEntryId = vehicleEntry.Id
                });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error processing entry");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("assign-space")]
        public async Task<IActionResult> AssignSpace([FromBody] ViewModels.EntryRequestViewModel request)
        {
            try
            {
                var availableSpace = await _context.ParkingSpaces
                    .Where(s => !s.IsOccupied && s.SpaceType == request.VehicleType)
                    .OrderBy(s => s.SpaceNumber)
                    .FirstOrDefaultAsync();

                if (availableSpace == null)
                {
                    return BadRequest(new { message = "No parking space available" });
                }

                var transaction = new Models.ParkingTransaction
                {
                    VehicleNumber = request.VehicleNumber,
                    VehicleLicensePlate = request.PlateNumber,
                    VehicleTypeId = request.VehicleType,
                    EntryTime = DateTime.UtcNow,
                    EntryGateId = request.GateId,
                    EntryPhotoPath = request.PhotoPath,
                    ParkingSpaceId = availableSpace.Id,
                    OperatorId = User.GetOperatorGuid(),
                    Status = "Active"
                };

                _context.ParkingTransactions.Add(transaction);
                
                availableSpace.IsOccupied = true;
                
                await _context.SaveChangesAsync();

                return Ok(new { 
                    transactionId = transaction.Id,
                    parkingSpaceId = availableSpace.Id 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning parking space");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("process-entry")]
        public async Task<IActionResult> ProcessEntry([FromBody] ViewModels.EntryRequestViewModel request)
        {
            try
            {
                // Validate input
                if (request == null)
                    return BadRequest("Invalid entry request");

                // If no image bytes, use empty byte array
                byte[] imageBytes = request.ImageBytes ?? new byte[0];
                string fileName = $"{Guid.NewGuid():N}_{request.PlateNumber}.jpg";
                
                string imagePath = await _storageService.SaveImage(fileName, imageBytes);
                
                var vehicleEntry = new Models.VehicleEntry
                {
                    VehicleNumber = request.VehicleNumber,
                    LicensePlate = request.PlateNumber,
                    VehicleType = request.VehicleType,
                    EntryTime = request.EntryTime,
                    VerifiedBy = request.EntryOperator ?? User.Identity?.Name,
                    ImagePath = imagePath,
                    Notes = request.TicketBarcode,
                    GateId = request.GateId,
                    VehicleId = Guid.NewGuid() // Temporary, will be updated or created
                };

                _context.VehicleEntries.Add(vehicleEntry);
                await _context.SaveChangesAsync();

                // Find or create vehicle
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.PlateNumber == request.PlateNumber);

                if (vehicle == null)
                {
                    vehicle = new Vehicle
                    {
                        PlateNumber = request.PlateNumber,
                        VehicleType = request.VehicleType.ToString(),
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };
                    _context.Vehicles.Add(vehicle);
                    await _context.SaveChangesAsync();
                }

                // Update vehicle entry with correct vehicle ID
                vehicleEntry.VehicleId = vehicle.Id;
                _context.VehicleEntries.Update(vehicleEntry);
                await _context.SaveChangesAsync();

                // Find available parking space
                var availableSpace = await _context.ParkingSpaces
                    .Where(s => !s.IsOccupied && s.Type == request.VehicleType)
                    .OrderBy(s => s.SpaceNumber)
                    .FirstOrDefaultAsync();

                if (availableSpace == null)
                {
                    return BadRequest(new { message = "No parking space available" });
                }

                // Create parking transaction
                var transaction = new Models.ParkingTransaction
                {
                    VehicleId = vehicle.Id,
                    VehicleNumber = request.VehicleNumber,
                    LicensePlate = request.PlateNumber,
                    VehicleType = request.VehicleType,
                    VehicleTypeId = 1, // Hardcoded for now
                    EntryTime = request.EntryTime,
                    EntryGateId = request.GateId,
                    EntryPhotoPath = imagePath,
                    ParkingSpaceId = availableSpace.Id,
                    OperatorId = User.GetOperatorGuid(),
                    Status = "Active",
                    TransactionNumber = Guid.NewGuid().ToString("N").Substring(0, 10),
                    CreatedAt = DateTime.Now,
                    ParkingZoneId = availableSpace.ParkingZoneId
                };

                _context.ParkingTransactions.Add(transaction);
                
                availableSpace.IsOccupied = true;
                _context.ParkingSpaces.Update(availableSpace);
                
                await _context.SaveChangesAsync();

                // Generate ticket
                var ticket = await _ticketService.GenerateTicketAsync(
                    request.VehicleNumber, 
                    request.VehicleType, 
                    imagePath, 
                    parkingGateId: request.GateId, 
                    operatorId: User.GetOperatorGuid()
                );

                return Ok(new { 
                    ticketNumber = ticket.TicketNumber,
                    imagePath = imagePath,
                    vehicleEntryId = vehicleEntry.Id,
                    transactionId = transaction.Id
                });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error processing entry");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessVehicleEntry([FromBody] ViewModels.VehicleEntryRequestViewModel request)
        {
            try
            {
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.LicensePlate == request.PlateNumber);

                if (vehicle == null)
                {
                    vehicle = new Vehicle
                    {
                        LicensePlate = request.PlateNumber,
                        VehicleType = request.VehicleType.ToString(),
                        EntryTime = request.EntryTime,
                        EntryOperator = request.EntryOperator,
                        PhotoEntry = request.PhotoEntry ?? Array.Empty<byte>(),
                        TicketBarcode = request.TicketBarcode ?? string.Empty
                    };
                    _context.Vehicles.Add(vehicle);
                }

                var operatorId = User.GetOperatorGuid();
                var vehicleEntry = new Models.VehicleEntry
                {
                    LicensePlate = request.PlateNumber,
                    VehicleType = request.VehicleType.ToString(),
                    EntryTime = request.EntryTime,
                    EntryOperator = request.EntryOperator,
                    PhotoEntry = request.PhotoEntry ?? Array.Empty<byte>(),
                    VehicleId = vehicle.Id,
                    ParkingSpaceId = request.ParkingSpaceId ?? Guid.Empty,
                    OperatorId = operatorId,
                    Notes = request.Notes ?? string.Empty
                };

                _context.VehicleEntries.Add(vehicleEntry);

                var transaction = new Models.ParkingTransaction
                {
                    VehicleId = vehicle.Id,
                    ParkingSpaceId = request.ParkingSpaceId ?? Guid.Empty,
                    EntryTime = request.EntryTime,
                    OperatorId = operatorId,
                    Vehicle = vehicle,
                    ParkingSpace = await _context.ParkingSpaces.FindAsync(request.ParkingSpaceId)
                };

                _context.ParkingTransactions.Add(transaction);

                await _context.SaveChangesAsync();

                var vehicleEntryViewModel = MapToVehicleEntryViewModel(request, operatorId);

                await _parkingHubContext.Clients.All.SendAsync("ReceiveVehicleEntry", vehicleEntryViewModel);
                
                if (request.PrintTicket)
                {
                    await _parkingHubContext.Clients.All.SendAsync("PrintTicket", new
                    {
                        plateNumber = request.PlateNumber,
                        vehicleType = request.VehicleType.ToString(),
                        entryTime = transaction.EntryTime,
                        transactionId = transaction.Id.ToString()
                    });
                }
                
                await _parkingHubContext.Clients.All.SendAsync("OpenEntryGate", request.GateId);

                return Ok(new 
                { 
                    VehicleEntry = vehicleEntryViewModel,
                    TransactionId = transaction.Id 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing vehicle entry");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessButtonPress(string gateId)
        {
            try
            {
                await _parkingHubContext.Clients.All.SendAsync("EntryButtonPressed", gateId);
                
                await _parkingHubContext.Clients.All.SendAsync("TriggerCamera", gateId);
                
                return Ok(new { success = true, message = "Push button processed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PrintTicket(int transactionId)
        {
            try
            {
                var transaction = await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .FirstOrDefaultAsync(t => t.Id == transactionId);

                if (transaction == null)
                {
                    return NotFound(new { success = false, message = "Transaksi tidak ditemukan" });
                }

                await _parkingHubContext.Clients.All.SendAsync("PrintTicket", new
                {
                    plateNumber = transaction.Vehicle?.LicensePlate,
                    vehicleType = (int)transaction.Vehicle?.Type,
                    entryTime = transaction.EntryTime,
                    transactionId = transaction.Id.ToString()
                });

                return Ok(new { success = true, message = "Print command sent" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleEntries(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? vehicleType = null)
        {
            try
            {
                var query = _context.VehicleEntries
                    .Include(ve => ve.Vehicle)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(vehicleType))
                {
                    query = query.Where(ve => ve.Vehicle.VehicleType == vehicleType);
                }

                var entries = await query
                    .OrderByDescending(ve => ve.EntryTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(ve => ConvertToViewModel(ve))
                    .ToListAsync();

                return Ok(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vehicle entries");
                return StatusCode(500, "An error occurred while retrieving vehicle entries");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Operator(string gateId = "GATE-01")
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var recentEntries = await _context.VehicleEntries
                .OrderByDescending(ve => ve.EntryTime)
                .Take(10)
                .Select(ve => ConvertToViewModel(ve))
                .ToListAsync();

            var viewModel = new ViewModels.EntryGateViewModel
            {
                GateId = gateId,
                OperatorName = $"{currentUser.FirstName} {currentUser.LastName}",
                Status = "Ready",
                IsCameraActive = false,
                IsPrinterActive = true,
                IsOfflineMode = false,
                LastSync = DateTime.Now,
                RecentEntries = recentEntries
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessVehicleEntryWithGuidConversion([FromBody] ViewModels.VehicleEntryRequestViewModel request)
        {
            try
            {
                // Convert string to VehicleType
                var vehicleType = Enum.TryParse<VehicleType>(request.VehicleType?.ToString() ?? "", out var parsedType) 
                    ? parsedType 
                    : VehicleType.Car;

                // Convert string/int to Guid
                var vehicleTypeId = request.VehicleTypeId is int intId 
                    ? new Guid(intId.ToString()) 
                    : request.VehicleTypeId is string strId && Guid.TryParse(strId, out var guidId) 
                        ? guidId 
                        : Guid.Empty;

                // Convert byte[] to string for photo
                var photoPath = request.PhotoEntry != null 
                    ? Convert.ToBase64String(request.PhotoEntry) 
                    : request.PhotoPath ?? string.Empty;

                var gateId = request.GateId ?? Guid.Empty;

                var transaction = new Models.ParkingTransaction
                {
                    VehicleId = Guid.NewGuid(), // Generate a new Guid for VehicleId
                    VehicleNumber = request.PlateNumber,
                    LicensePlate = request.PlateNumber,
                    VehicleType = vehicleType.ToString(),
                    VehicleTypeId = vehicleTypeId,
                    EntryTime = request.EntryTime,
                    EntryGateId = gateId,
                    EntryPhotoPath = photoPath,
                    ParkingSpaceId = request.ParkingSpaceId ?? Guid.Empty,
                    OperatorId = User.GetOperatorGuid(),
                    Status = "Active"
                };

                _context.ParkingTransactions.Add(transaction);

                var vehicleEntry = new Models.VehicleEntry
                {
                    Id = Guid.NewGuid(),
                    VehicleId = transaction.VehicleId,
                    EntryTime = transaction.EntryTime,
                    ImagePath = photoPath,
                    PlateImagePath = photoPath,
                    IsPlateVerified = false,
                    VehicleNumber = transaction.VehicleNumber,
                    LicensePlate = transaction.LicensePlate,
                    VehicleType = transaction.VehicleType,
                    GateId = transaction.EntryGateId,
                    Notes = request.Notes ?? "Automatic entry"
                };

                _context.VehicleEntries.Add(vehicleEntry);

                var operatorId = Guid.TryParse(transaction.OperatorId.ToString(), out Guid parsedOperatorId) 
                    ? parsedOperatorId 
                    : Guid.Empty;

                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing vehicle entry with Guid conversion");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessVehicleEntryListConversion([FromBody] List<ViewModels.VehicleEntryRequestViewModel> requests)
        {
            try
            {
                var vehicleEntries = requests.Select(model => new Models.VehicleEntry
                {
                    Id = Guid.NewGuid(),
                    VehicleId = model.VehicleId,
                    EntryTime = model.EntryTime,
                    LicensePlate = model.PlateNumber,
                    VehicleType = model.VehicleType.ToString(),
                    GateId = model.GateId ?? Guid.Empty
                }).ToList();

                _context.VehicleEntries.AddRange(vehicleEntries);

                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing vehicle entry list conversion");
                return StatusCode(500, ex.Message);
            }
        }
    }
}