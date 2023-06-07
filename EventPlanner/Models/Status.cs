using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EventPlanner.Areas.Identity.Data;

namespace EventPlanner.Models
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        public bool Interested { get; set; }

        [Required]
        public bool Going { get; set; }

        // Navigation property's
        [Required]
        public string UserId { get; set; }
        public CustomUser User { get; set; }

        [Required]
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
