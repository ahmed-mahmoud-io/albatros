using System.ComponentModel.DataAnnotations;

namespace Albatros.Models
{
    public class ApplicationUser
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }

        public string Role { get; set; } = "User";

        public bool IsEmailVerified { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpiration { get; set; }

        public string? GoogleId { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public ICollection<Property>? Properties { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<VisitRequest>? VisitRequests { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
    }
}