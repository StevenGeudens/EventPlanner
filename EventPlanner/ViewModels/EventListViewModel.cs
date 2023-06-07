using EventPlanner.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EventPlanner.ViewModels
{
    public class EventListViewModel
    {
        public string EventSearch { get; set; }
        public List<Event> Events { get; set; }
        public SelectList Categories { get; set; }
    }
}
