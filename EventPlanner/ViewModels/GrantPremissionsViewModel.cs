using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventPlanner.ViewModels
{
    public class GrantPremissionsViewModel
    {
        public SelectList Users { get; set; }
        public SelectList Roles { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}
