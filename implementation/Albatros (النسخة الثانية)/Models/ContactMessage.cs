using System.ComponentModel.DataAnnotations;

namespace Albatros.Models
{
    public class ContactMessage
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        [Required, MaxLength(300)]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}
