
using Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    /// <summary>
    /// Participant class that extends Person
    /// </summary>
    public class Participant : Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public RoomType RoomType { get; set; }

        // Foreign Key
        public string? ApplicationUserId { get; set; }

        // Navigation Property
        [ForeignKey(nameof(ApplicationUserId))]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }


        // Foreign Key
        [Required]
        public int BookingId { get; set; }

        // Navigation Property
        [ValidateNever]
        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; }
    }
}
