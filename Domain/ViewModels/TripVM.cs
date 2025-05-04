using Domain.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Domain.ViewModels
{
    /// <summary>
    /// View Model for the Trip
    /// </summary>
    public class TripVM
    {
        [ValidateNever]
        public Trip Trip { get; set; }
        [ValidateNever]
        public Itinerary Itinerary { get; set; }

        [ValidateNever]
        public Booking Booking { get; set; }

        [ValidateNever]
        public List<Itinerary> ItineraryList { get; set; }

        [ValidateNever]
        public List<Review> ReviewList { get; set; }

    }
}
