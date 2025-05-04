using Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class NotificationPreference
    {
        public int Id { get; set; }

        public NotificationType Type { get; set; }

        public bool ByEmail { get; set; }

        public bool InApp { get; set; }

        // Foreign key
        public string ApplicationUserId { get; set; }

        // Navigation property
        [ForeignKey(nameof(ApplicationUserId))]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }

    }
}
