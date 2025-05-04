using Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    /// <summary>
    /// Payment class
    /// </summary>
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public double? Amount { get; set; }

        public PaymentStatus Status { get; set; }


        public DateTime PaymentDate { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        // Foreign Key
        [Required]
        public int BookingId { get; set; }

        // Navigation Property
        [ValidateNever]
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }

        // Foreign Key
        [Required]
        public string ApplicationUserId { get; set; }

        // Navigation Property
        [ValidateNever]
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
