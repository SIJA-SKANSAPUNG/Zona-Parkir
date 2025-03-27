using System;

namespace Parking_Zone.ViewModels
{
    public abstract class BaseViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
