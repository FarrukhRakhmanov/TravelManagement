
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    /// <summary>
    /// ApplicationUser class that extends IdentityUser
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [ValidateNever]
        public ICollection<Trip> Trips { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Returns the full name of the user
        /// </summary>
        /// <returns></returns>
        public string FullName()
        {
            return $"{FirstName} {LastName}";
        }

        //public int? UserNotificationPreferenceId { get; set; }

        //[ForeignKey(nameof(UserNotificationPreferenceId))]
        //public UserNotificationPreference? UserNotificationPreference { get; set; }
    }

}
