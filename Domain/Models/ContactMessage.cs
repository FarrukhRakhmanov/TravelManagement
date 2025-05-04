using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    /// <summary>
    /// Represents a contact message from a user.
    /// </summary>
    public class ContactMessage : Person
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        public required string Message { get; set; }

        [Required]
        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
