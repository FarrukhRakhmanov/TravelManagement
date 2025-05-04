using Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    /// <summary>
    /// Trip model
    /// </summary>
    public class Trip
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        public DateOnly? StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Required]
        public DateOnly? EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Required]
        public string Description { get; set; }

        [Required]
        public string Includes { get; set; }

        [Required]
        public string Excludes { get; set; }

        [Required] public double DiscountPricePerPerson { get; set; } = 0.00;

        public double? OriginalPricePerPerson { get; set; }

        [Required]
        public double SingleSupplement { get; set; }
        [Required]
        public TripStatus Status { get; set; }
        public int SeatsAvailable { get; set; }
        public int NumberOfDays { get; set; }

        public bool IsFeatured { get; set; } = false;
        public TripStyle Style { get; set; }
        public string? ImageUrl { get; set; }

        [ValidateNever]
        public bool? HasReviewSent { get; set; } = false;


        // Foreign Key
        [Required]
        public string ApplicationUserId { get; set; }

        // Navigation Property
        [ValidateNever]
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser? ApplicationUser { get; set; }

        // Collections of related entities
        public ICollection<Itinerary>? Itineraries { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}
