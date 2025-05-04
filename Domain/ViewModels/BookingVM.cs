using Domain.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Domain.ViewModels
{
    /// <summary>
    /// View model for booking and participant
    /// </summary>
    public class BookingVM
    {
        [ValidateNever]
        public required Booking Booking { get; set; }
        [ValidateNever]
        public required Participant Participant { get; set; }
    }
}
