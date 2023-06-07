using EventPlanner.Areas.Identity.Data;
using EventPlanner.Models;
using System;

namespace EventPlanner.ViewModels
{
    public class ReviewDetailViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public CustomUser User { get; set; }
        public Event Event { get; set; }
    }
}
