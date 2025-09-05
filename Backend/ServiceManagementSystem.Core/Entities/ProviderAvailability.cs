using System.ComponentModel.DataAnnotations;

namespace ServiceManagementSystem.Core.Entities
{
    public class ProviderAvailability
    {
        public int Id { get; set; }
        
        [Required]
        public int ProviderId { get; set; }
        
        [Required]
        public DayOfWeek DayOfWeek { get; set; }
        
        [Required]
        public TimeSpan StartTime { get; set; }
        
        [Required]
        public TimeSpan EndTime { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual Provider Provider { get; set; } = null!;
    }
}
