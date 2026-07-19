using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albatros.Models
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public int Bedrooms { get; set; }

        [Required]
        public int Bathrooms { get; set; }

        [Required]
        public double Area { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public PropertyStatus Status { get; set; } = PropertyStatus.Pending;

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        public ICollection<PropertyImage>? Images { get; set; }

        public ICollection<Favorite>? Favorites { get; set; }

        public ICollection<Review>? Reviews { get; set; }

        public ICollection<VisitRequest>? VisitRequests { get; set; }
    }
}