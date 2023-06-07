using EventPlanner.Areas.Identity.Data;
using EventPlanner.Models;
using System.Collections.Generic;

namespace EventPlanner.ViewModels
{
    public class EventsListViewModel
    {
        public List<Event> Events { get; set; }
    }
}
