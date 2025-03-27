using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Parking_Zone.Models;
using Parking_Zone.Services;
using System;
using System.Threading.Tasks;

namespace Parking_Zone.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionsApiController : ControllerBase
    {
        private readonly IParkingTransactionService _transactionService;
        private readonly ILogger<TransactionsApiController> _logger;

        public TransactionsApiController(
            IParkingTransactionService transactionService,
            ILogger<TransactionsApiController> logger)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var transaction = await _transactionService.CreateTransactionAsync(dto.VehicleId, dto.ParkingZoneId);
                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(Guid id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(id);
                if (transaction == null)
                    return NotFound();

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction {TransactionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions([FromQuery] TransactionFilterDto filter)
        {
            try
            {
                if (filter.VehicleId.HasValue)
                {
                    var vehicleTransactions = await _transactionService.GetTransactionsByVehicleAsync(filter.VehicleId.Value);
                    return Ok(vehicleTransactions);
                }

                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    var dateRangeTransactions = await _transactionService.GetTransactionsByDateRangeAsync(
                        filter.StartDate.Value, filter.EndDate.Value);
                    return Ok(dateRangeTransactions);
                }

                if (filter.ActiveOnly)
                {
                    var activeTransactions = await _transactionService.GetActiveTransactionsAsync();
                    return Ok(activeTransactions);
                }

                var allTransactions = await _transactionService.GetAllTransactionsAsync();
                return Ok(allTransactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/fee")]
        public async Task<IActionResult> CalculateFee(Guid id)
        {
            try
            {
                var fee = await _transactionService.CalculateParkingFeeAsync(id);
                return Ok(new { Fee = fee });
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
                _logger.LogError(ex, "Error calculating fee for transaction {TransactionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteTransaction(Guid id, [FromBody] CompleteTransactionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var transaction = await _transactionService.CompleteTransactionAsync(id, dto.Amount);
                return Ok(transaction);
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
                _logger.LogError(ex, "Error completing transaction {TransactionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class CreateTransactionDto
    {
        public Guid VehicleId { get; set; }
        public Guid ParkingZoneId { get; set; }
    }

    public class CompleteTransactionDto
    {
        public decimal Amount { get; set; }
    }

    public class TransactionFilterDto
    {
        public Guid? VehicleId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool ActiveOnly { get; set; }
    }
}
