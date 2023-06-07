using EventPlanner.Areas.Identity.Data;
using EventPlanner.Data;
using EventPlanner.Data.UnitOfWork;
using EventPlanner.Models;
using EventPlanner.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace EventPlanner.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<CustomUser> _userManager;

        public UserController(IUnitOfWork unitOfWork, UserManager<CustomUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            CustomUser user = _userManager.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Favorites)
                .Include(u => u.Statuses)
                .Include(u => u.Collaborations)
                .FirstOrDefault();

            UserEventListViewModel vm = new UserEventListViewModel
            {
                Events = new List<Event>()
            };

            // Get the users favorite events
            user.Favorites.ForEach(favorite => vm.Events.Add(_unitOfWork.EventRepo.Get(e => e.EventId.Equals(favorite.EventId), e => e.Category).FirstOrDefault()));
            // Get the users status events (going & interested)
            foreach(Status status in user.Statuses)
            {
                Event statusEvent = _unitOfWork.EventRepo.Get(e => e.EventId.Equals(status.EventId), e => e.Category).FirstOrDefault();
                if(!vm.Events.Contains(statusEvent)) // Prevent double events
                    vm.Events.Add(statusEvent);
            }
            // Get the users collaborated events where he/she is organizer
            foreach(Collaboration collaboration in user.Collaborations)
            {
                if (collaboration.Organizer)
                {
                    Event collabEvent = _unitOfWork.EventRepo.Get(e => e.EventId.Equals(collaboration.EventId), e => e.Category).FirstOrDefault();
                    if (!vm.Events.Contains(collabEvent)) // Prevent double events
                        vm.Events.Add(collabEvent);
                }
            }
            return View(vm);
        }

        public IActionResult Filter(string filter)
        {

            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            CustomUser user = _userManager.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Favorites)
                .Include(u => u.Statuses)
                .Include(u => u.Collaborations)
                .FirstOrDefault();

            UserEventListViewModel vm = new UserEventListViewModel()
            {
                Events = new List<Event>()
            };

            if (string.IsNullOrEmpty(filter) || (filter != "Favorite" && filter != "Interested" && filter != "Going" && filter != "My events" && filter != "All"))
            {
                filter = "All"; // If the filter does not match any of the above retur all events
            }

            if (filter == "Favorite" || filter == "All")
            {
                // Get the users favorite events
                user.Favorites.ForEach(favorite => vm.Events.Add(_unitOfWork.EventRepo.Get(e => e.EventId.Equals(favorite.EventId), e => e.Category).FirstOrDefault()));
            }
            if (filter == "Interested" || filter == "All")
            {
                // Get the users status events where the user is interested
                foreach (Status status in user.Statuses)
                {
                    if (status.Interested)
                    {
                        Event statusEvent = _unitOfWork.EventRepo.Get(e => e.EventId.Equals(status.EventId), e => e.Category).FirstOrDefault();
                        if (!vm.Events.Contains(statusEvent)) // Prevent double events (only when "All" is executed)
                            vm.Events.Add(statusEvent);
                    }
                }
            }
            if (filter == "Going" || filter == "All")
            {
                // Get the users status events where the user is going
                foreach (Status status in user.Statuses)
                {
                    if (status.Going)
                    {
                        Event statusEvent = _unitOfWork.EventRepo.Get(e => e.EventId.Equals(status.EventId), e => e.Category).FirstOrDefault();
                        if (!vm.Events.Contains(statusEvent)) // Prevent double events (only when "All" is executed)
                            vm.Events.Add(statusEvent);
                    }
                }
            }
            if (filter == "My events" || filter == "All")
            {
                // Get the users collaborated events where he/she is organizer aka. My events
                foreach (Collaboration collaboration in user.Collaborations)
                {
                    if (collaboration.Organizer) 
                    {
                        Event collaborationEvent = _unitOfWork.EventRepo.Get(e => e.EventId.Equals(collaboration.EventId), e => e.Category).FirstOrDefault();
                        if (!vm.Events.Contains(collaborationEvent))
                            vm.Events.Add(_unitOfWork.EventRepo.Get(e => e.EventId.Equals(collaboration.EventId), e => e.Category).FirstOrDefault());

                    }
                }
            }
            return PartialView("_ShowMyEventsPartial", vm);
        }
    }
}
