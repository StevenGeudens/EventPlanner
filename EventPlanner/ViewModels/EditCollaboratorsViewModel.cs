using EventPlanner.Areas.Identity.Data;
using EventPlanner.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EventPlanner.ViewModels
{
    public class EditCollaboratorsViewModel
    {
        public Event Event { get; set; }
        public int? EventId { get; set; }
        public SelectList Users { get; set; }
        public string CurrentUserId { get; set; }
        public List<Collaboration> Collaborations { get; set; }
        public string SelectedUserId { get; set; }
        public bool Organizer { get; set; }
    }
}
