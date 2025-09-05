using System.ComponentModel.DataAnnotations;

namespace ServiceManagementSystem.Core.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int ProviderId { get; set; }
        
        [Required]
        public int ServiceId { get; set; }
        
        [Required]
        public DateTime BookingDate { get; set; }
        
        [Required]
        public TimeSpan StartTime { get; set; }
        
        [Required]
        public TimeSpan EndTime { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? SpecialInstructions { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, InProgress, Completed, Cancelled, Rejected
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? ConfirmedAt { get; set; }
        
        public DateTime? CompletedAt { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Provider Provider { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
        public virtual Payment? Payment { get; set; }
    }
}
