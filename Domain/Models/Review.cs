using Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    /// <summary>
    /// Review class
    /// </summary>
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Comment { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public ReviewStatus Status { get; set; }

        // Foreign Key
        [Required]
        public int TripId { get; set; }

        // Navigation Property
        [ValidateNever]
        [ForeignKey(nameof(TripId))]
        public Trip Trip { get; set; }

        // Foreign Key
        [Required]
        public int ParticipantId { get; set; }

        // Navigation Property
        [ValidateNever]
        [ForeignKey(nameof(ParticipantId))]
        public Participant Participant { get; set; }
    }
}
