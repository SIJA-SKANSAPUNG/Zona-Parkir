using System;
using System.Collections.Generic;

namespace Parking_Zone.ViewModels
{
    public class GateExitOperationalViewModel : BaseViewModel
    {
        public Guid GateId { get; set; }
        public string OperatorName { get; set; } = null!;
        public string Status { get; set; } = "Idle";
        public DateTime LastSync { get; set; } = DateTime.UtcNow;
        public bool IsCameraActive { get; set; }
        public List<object> RecentExits { get; set; } = new List<object>();
    }
}
