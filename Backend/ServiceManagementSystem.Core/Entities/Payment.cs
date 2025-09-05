using System.ComponentModel.DataAnnotations;

namespace ServiceManagementSystem.Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        
        [Required]
        public int BookingId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "CreditCard"; // CreditCard, DebitCard, PayPal, etc.
        
        [Required]
        [StringLength(100)]
        public string TransactionId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed, Refunded
        
        [StringLength(500)]
        public string? FailureReason { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ProcessedAt { get; set; }
        
        // Navigation properties
        public virtual Booking Booking { get; set; } = null!;
    }
}
