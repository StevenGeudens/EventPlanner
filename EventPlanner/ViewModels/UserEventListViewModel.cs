using EventPlanner.Areas.Identity.Data;
using EventPlanner.Models;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Collections.Generic;

namespace EventPlanner.ViewModels
{
    public class UserEventListViewModel
    {
        public string Filter { get; set; }
        public List<Event> Events { get; set; }
    }
}
