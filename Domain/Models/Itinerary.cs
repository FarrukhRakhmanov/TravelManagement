using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    /// <summary>
    /// Itinerary model
    /// </summary>
    public class Itinerary
    {
        [Key]
        public int Id { get; set; }

        public DateOnly? Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Required]
        public string Activity { get; set; }
        public string Description { get; set; }
        public string? Includes { get; set; }
        public string? Excludes { get; set; }
        public string? ImageUrl { get; set; }

        public int Order { get; set; } = 0;

        // Foreign key reference to the Trip table
        [Required]
        public int TripId { get; set; }

        // Navigation property
        [ForeignKey("TripId")]
        [ValidateNever]
        public Trip Trip { get; set; }

        /// <summary>
        /// Fixes date formatting
        /// </summary>
        /// <returns>date formatted to string: 25 December 2025</returns>
        public string DateToString()
        {
            if (Date.HasValue)
            {
                return Date.Value.ToString("dd MMMM yyyy");
            }
            return string.Empty;
        }
    }


}
