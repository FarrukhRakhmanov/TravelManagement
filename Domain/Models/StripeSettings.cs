using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    /// <summary>
    /// Represents the Stripe settings
    /// </summary>
    public class StripeSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string SecretKey { get; set; }

        [Required]
        public required string PublishableKey { get; set; }
    }
}
