using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using EventPlanner.Areas.Identity.Data;

namespace EventPlanner.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required(ErrorMessage = "Please provide a review title")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Please provide a rating")]
        public int Rating { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        // Navigation property's
        [Required]
        public string UserId { get; set; }
        public CustomUser User { get; set; }

        [Required]
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
