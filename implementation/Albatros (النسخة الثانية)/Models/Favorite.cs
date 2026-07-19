using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albatros.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }

        public int UserId { get; set; }

        public int PropertyId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public Property Property { get; set; }
    }
}