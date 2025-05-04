using Domain.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    /// <summary>
    /// Notification class
    /// </summary>
    public class Notification
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Message { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool IsRead { get; set; }

        public NotificationType Type { get; set; }

        // Foreign key
        public required string ApplicationUserId { get; set; }

        // Navigation property
        [ForeignKey(nameof(ApplicationUserId))]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }

    }

}
