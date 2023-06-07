using EventPlanner.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace EventPlanner.Areas.Identity.Data
{
    public partial class CustomUser : IdentityUser
    {
        [Required, PersonalData]
        public string Name { get; set; }

        [Display(Name = "First name")]
        [Required, PersonalData]
        public string FirstName { get; set; }

        [PersonalData, DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        [PersonalData]
        public Gender Gender { get; set; }

        // Navigation property's
        public List<Favorite> Favorites { get; set; }
        public List<Status> Statuses { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Collaboration> Collaborations { get; set; }
    }
}
