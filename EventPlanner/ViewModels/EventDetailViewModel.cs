using EventPlanner.Areas.Identity.Data;
using EventPlanner.Models;
using System;
using System.Collections.Generic;

namespace EventPlanner.ViewModels
{
    public class EventDetailViewModel
    {
        public string CurrentUserId { get; set; }

        public int EventId { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string PostalCode { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<Collaboration> Collaborations { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Favorite> Favorites { get; set; }
        public List<Status> Statuses { get; set; }
        public int PeopleGoing { get; set; }
        public int PeopleInterested { get; set; }

        // User Id's of the Organizers of this event (give edit rights)
        public List<string> OrganizerIds { get; set; }
    }
}
