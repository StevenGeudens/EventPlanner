using EventPlanner.Models;
using System.ComponentModel.DataAnnotations;

namespace EventPlanner.ViewModels
{
    public class CreateReviewViewModel
    {
        public int EventId { get; set; }
        public string CurrentUserId { get; set; }
        public Event Event { get; set; }

        [Required(ErrorMessage = "Please enter a review title")]
        public string Title { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter a rating")]
        public int? Rating { get; set; }
    }
}
