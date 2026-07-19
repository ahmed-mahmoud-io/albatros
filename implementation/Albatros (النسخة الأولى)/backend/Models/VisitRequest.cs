using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albatros.Models
{
    public class VisitRequest
    {
        [Key]
        public int RequestId { get; set; }

        public DateTime VisitDate { get; set; }

        public VisitRequestStatus Status { get; set; } = VisitRequestStatus.Pending;

        public int UserId { get; set; }

        public int PropertyId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public Property Property { get; set; }
    }
}