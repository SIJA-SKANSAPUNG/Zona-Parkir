using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Parking_Zone.Models;

namespace Parking_Zone.ViewModels
{
    public class VehicleHistoryPageViewModel
    {
        public VehicleInfo Vehicle { get; set; }
        public List<VehicleHistoryEntry> History { get; set; }
        public VehicleHistoryFilter Filter { get; set; }
        public VehicleStatistics Statistics { get; set; }
        public PaginationInfo Pagination { get; set; }

        public VehicleHistoryPageViewModel()
        {
            Vehicle = new VehicleInfo();
            History = new List<VehicleHistoryEntry>();
            Filter = new VehicleHistoryFilter();
            Statistics = new VehicleStatistics();
            Pagination = new PaginationInfo();
        }
    }

    public class VehicleInfo
    {
        public string LicensePlate { get; set; }
        public string VehicleType { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
        public int TotalVisits { get; set; }
        public decimal TotalSpent { get; set; }
        public TimeSpan AverageStayDuration { get; set; }
        public string PreferredGate { get; set; }
        public string Notes { get; set; }
    }

    public class VehicleHistoryEntry
    {
        public int TransactionId { get; set; }
        public string TicketNumber { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public TimeSpan? Duration { get; set; }
        public string EntryGate { get; set; }
        public string ExitGate { get; set; }
        public decimal Fee { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }

    public class VehicleHistoryFilter
    {
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Gate")]
        public int? GateId { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Sort By")]
        public string SortBy { get; set; } = "EntryTime";

        [Display(Name = "Sort Direction")]
        public string SortDirection { get; set; } = "DESC";

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class VehicleStatistics
    {
        public int TotalVisits { get; set; }
        public decimal TotalRevenue { get; set; }
        public TimeSpan AverageStayDuration { get; set; }
        public Dictionary<string, int> VisitsByGate { get; set; }
        public Dictionary<string, decimal> RevenueByMonth { get; set; }
        public List<TimeSlot> PreferredTimeSlots { get; set; }

        public VehicleStatistics()
        {
            VisitsByGate = new Dictionary<string, int>();
            RevenueByMonth = new Dictionary<string, decimal>();
            PreferredTimeSlots = new List<TimeSlot>();
        }
    }

    public class TimeSlot
    {
        public string TimeRange { get; set; }
        public int VisitCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
} 