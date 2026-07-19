using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Albatros.Models
{
    public class PropertyImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public int PropertyId { get; set; }

        [ForeignKey("PropertyId")]
        public Property Property { get; set; }
    }
}
