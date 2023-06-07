using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
    public partial class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please enter a category name")]
        public string Name { get; set; }

        // Navigation property's
        public List<Event> Events { get; set; }
    }
}
