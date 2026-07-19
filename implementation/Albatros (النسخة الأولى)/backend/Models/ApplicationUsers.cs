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

        [Required]
        public string Password { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        // "User" or "Admin" -- controls back-office access
        public string Role { get; set; } = "User";

        // "Buyer" or "Seller" -- set at registration, shown throughout the site/admin
        public string UserType { get; set; } = "Buyer";

        public ICollection<Property>? Properties { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<VisitRequest>? VisitRequests { get; set; }
    }
}
