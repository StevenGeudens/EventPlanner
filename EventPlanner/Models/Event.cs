using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace EventPlanner.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string Town { get; set; }

        [Required]
        public string Country { get; set; }

        public string Description { get; set; }

        public string Thumbnail { get; set; }

        // Navigation property's
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<Collaboration> Collaborations { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Status> Statuses { get; set; }
        public List<Favorite> Favorites { get; set; }
    }
}
