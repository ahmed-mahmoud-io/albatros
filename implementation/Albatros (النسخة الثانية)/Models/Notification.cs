using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albatros.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required, MaxLength(500)]
        public string Message { get; set; }

        [MaxLength(300)]
        public string? LinkUrl { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
