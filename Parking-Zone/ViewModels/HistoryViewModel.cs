using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Parking_Zone.Models;

namespace Parking_Zone.ViewModels
{
    public class HistoryViewModel
    {
        public List<TransactionHistoryItem> Transactions { get; set; }
        public HistoryFilterModel Filter { get; set; }
        public HistorySummaryModel Summary { get; set; }

        public HistoryViewModel()
        {
            Transactions = new List<TransactionHistoryItem>();
            Filter = new HistoryFilterModel();
            Summary = new HistorySummaryModel();
        }
    }

    public class TransactionHistoryItem
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public string VehicleType { get; set; }
        public string LicensePlate { get; set; }
        public string EntryGate { get; set; }
        public string ExitGate { get; set; }
        public string EntryOperator { get; set; }
        public string ExitOperator { get; set; }
        public decimal Fee { get; set; }
        public string Status { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Notes { get; set; }
    }

    public class HistoryFilterModel
    {
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        [Display(Name = "License Plate")]
        public string LicensePlate { get; set; }

        [Display(Name = "Gate")]
        public int? GateId { get; set; }

        [Display(Name = "Operator")]
        public int? OperatorId { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Sort By")]
        public string SortBy { get; set; }

        [Display(Name = "Sort Direction")]
        public string SortDirection { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class HistorySummaryModel
    {
        public int TotalTransactions { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalVehicles { get; set; }
        public Dictionary<string, int> VehicleTypeDistribution { get; set; }
        public Dictionary<string, decimal> RevenueByGate { get; set; }
        public decimal AverageStayDuration { get; set; }
        public decimal PeakHourOccupancy { get; set; }

        public HistorySummaryModel()
        {
            VehicleTypeDistribution = new Dictionary<string, int>();
            RevenueByGate = new Dictionary<string, decimal>();
        }
    }
} 