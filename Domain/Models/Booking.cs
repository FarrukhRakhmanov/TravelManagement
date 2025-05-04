using Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    /// <summary>
    /// Booking class
    /// </summary>
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public double? TotalAmount { get; set; }
        public BookingStatus? Status { get; set; }

        public int NumberOfParticipants { get; set; }

        // Foreign Keys
        [Required]
        public int TripId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        // Navigation Properties
        [ValidateNever]
        [ForeignKey("TripId")]
        public Trip Trip { get; set; }

        [ValidateNever]
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }

        // Represents the participants in the booking
        public ICollection<Participant>? Participants { get; set; }
    }
}
