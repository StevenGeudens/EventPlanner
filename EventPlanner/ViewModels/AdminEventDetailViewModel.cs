using EventPlanner.Models;
using System;
using System.Collections.Generic;

namespace EventPlanner.ViewModels
{
    public class AdminEventDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string PostalCode { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
        public Category Category { get; set; }
        public string Thumbnail { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
