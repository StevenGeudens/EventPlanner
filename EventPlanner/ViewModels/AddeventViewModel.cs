using EventPlanner.Areas.Identity.Data;
using EventPlanner.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EventPlanner.ViewModels
{
    public class AddeventViewModel
    {
        // New event

        [Required(ErrorMessage = "Please enter a event name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int SelectedCategoryId { get; set; }

        [Required(ErrorMessage = "Please enter a street name")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Please provide a house number")]
        [RegularExpression(@"^[1-9]\d*(?:[ -]?(?:[a-zA-Z]+|[1-9]\d*))?$", ErrorMessage = "Please provide a valid house number")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Please provide a postal code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Please provide a town")]
        public string Town { get; set; }

        [Required(ErrorMessage = "Please select a country")]
        public string SelectedCountry { get; set; }

        [Required(ErrorMessage = "Please provide a start time")]
        [DisplayName("Start date and time")]
        public DateTime Start { get; set; }

        [Required(ErrorMessage = "Please provide a end time")]
        [DisplayName("End date and time")]
        public DateTime End { get; set; }
        public IFormFile Thumbnail { get; set; }
        public string Description { get; set; }

        // default Collaboration
        public Collaboration Collaboration { get; set; }

        // New collaboration
        public ObservableCollection<CustomUser> Users { get; set; }
        public string CollabUserId { get; set; }

        // Form info
        public SelectList Categories { get; set; }
        public SelectList Countries { get; set; }
    }
}
