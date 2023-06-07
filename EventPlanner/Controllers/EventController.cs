using EventPlanner.Areas.Identity.Data;
using EventPlanner.Data;
using EventPlanner.Data.UnitOfWork;
using EventPlanner.Models;
using EventPlanner.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventPlanner.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private IWebHostEnvironment _enviroment;
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<CustomUser> _userManager;

        public EventController(IUnitOfWork unitOfWork, IWebHostEnvironment environment, UserManager<CustomUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _enviroment= environment;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Index(int? id)
        {
            Event evnt = _unitOfWork.EventRepo.Get(
                e => e.EventId == id,
                e => e.Collaborations,
                e => e.Reviews,
                e => e.Statuses,
                e => e.Category,
                e => e.Favorites)
                .FirstOrDefault();

            if (evnt != null)
            {
                // Get each collaborations user
                evnt.Collaborations.ForEach(c => c.User = _userManager.Users.Where(u => u.Id == c.UserId).FirstOrDefault());
                // Get each reviews user
                evnt.Reviews.ForEach(r => r.User = _userManager.Users.Where(u => u.Id == r.UserId).FirstOrDefault());

                EventDetailViewModel vm = new EventDetailViewModel()
                {
                    CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    EventId = evnt.EventId,
                    Name = evnt.Name,
                    Category = evnt.Category,
                    Street = evnt.Street,
                    Number = evnt.Number,
                    PostalCode = evnt.PostalCode,
                    Town = evnt.Town,
                    Country = evnt.Country,
                    Description = evnt.Description,
                    Thumbnail = evnt.Thumbnail,
                    Start = evnt.Start,
                    End = evnt.End,
                    Collaborations = evnt.Collaborations,
                    Reviews = evnt.Reviews,
                    Favorites = evnt.Favorites,
                    Statuses = evnt.Statuses,
                    PeopleGoing = evnt.Statuses.Where(s => s.Going == true).Count(),
                    PeopleInterested = evnt.Statuses.Where(s => s.Interested == true).Count(),
                    OrganizerIds = new List<string>()
                };
                foreach (Collaboration collaboration in evnt.Collaborations)
                {
                    if (collaboration.Organizer) vm.OrganizerIds.Add(collaboration.UserId);
                }
                return View(vm);
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ToggleFavorite(int? eventId, string userId)
        {
            if (eventId != null && !string.IsNullOrEmpty(userId))
            {
                if (userId == User.FindFirstValue(ClaimTypes.NameIdentifier)) // Check that the user is not editing some other users favorite
                {
                    Favorite favorite = _unitOfWork.FavoriteRepo.Get(f => f.EventId == eventId && f.UserId == userId).FirstOrDefault();
                    if (favorite == null)
                        _unitOfWork.FavoriteRepo.Add(new Favorite() { EventId = (int)eventId, UserId = userId });
                    else
                        _unitOfWork.FavoriteRepo.Delete(favorite);
                    await _unitOfWork.SaveAsync();
                }
            }
            return PartialView("_ToggleFavoritePartial", new EventDetailViewModel() { Favorites = _unitOfWork.FavoriteRepo.Get(f => f.EventId == eventId).ToList(), CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) });
        }
        public async Task<IActionResult> ToggleInterestedStatus(int? eventId, string userId)
        {
            if (eventId != null && !string.IsNullOrEmpty(userId))
            {
                if (userId == User.FindFirstValue(ClaimTypes.NameIdentifier)) // Check that the user is not editing some other users status
                {
                    Status status = _unitOfWork.StatusRepo.Get(s => s.EventId == eventId && s.UserId == userId).FirstOrDefault();
                    if (status == null)
                    {
                        status = new Status() { EventId = (int)eventId, UserId = userId, Interested = true, Going = false };
                        _unitOfWork.StatusRepo.Add(status);
                    }
                    else
                    {
                        if (status.Interested)
                            _unitOfWork.StatusRepo.Delete(status);
                        else
                        {
                            status.Interested = true;
                            status.Going = false;
                            _unitOfWork.StatusRepo.Update(status);
                        }
                    }
                    await _unitOfWork.SaveAsync();
                }
            }
            return PartialView("_ToggleInterestedParital", new EventDetailViewModel() { Statuses = _unitOfWork.StatusRepo.Get(s => s.EventId == eventId).ToList(), CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) });
        }
        public async Task<IActionResult> ToggleGoingStatus(int? eventId, string userId)
        {
            if (eventId != null && !string.IsNullOrEmpty(userId))
            {
                if (userId == User.FindFirstValue(ClaimTypes.NameIdentifier)) // Check that the user is not editing some other users status
                {
                    Status status = _unitOfWork.StatusRepo.Get(s => s.EventId == eventId && s.UserId == userId).FirstOrDefault();
                    if (status == null)
                    {
                        status = new Status() { EventId = (int)eventId, UserId = userId, Interested = false, Going = true };
                        _unitOfWork.StatusRepo.Add(status);
                    }
                    else
                    {
                        if (status.Going)
                            _unitOfWork.StatusRepo.Delete(status);
                        else
                        {
                            status.Interested = false;
                            status.Going = true;
                            _unitOfWork.StatusRepo.Update(status);
                        }
                    }
                    await _unitOfWork.SaveAsync();
                }
            }
            return PartialView("_ToggleGoingPartial", new EventDetailViewModel() { Statuses = _unitOfWork.StatusRepo.Get(s => s.EventId == eventId).ToList(), CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) });
        }

        public IActionResult Create()
        {
            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            CustomUser user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();

            AddeventViewModel vm = new AddeventViewModel()
            {
                Categories = new SelectList(_unitOfWork.CategoryRepo.Get(), "CategoryId", "Name"),
                Countries = new SelectList(new List<string> { "Belgium", "Netherlands", "Luxembourg" }),
                Collaboration = new Collaboration() // Add a default collaboration for the user that is creating the event
                { 
                    Organizer = true, 
                    UserId = user.Id, 
                    User = user
                },
                // AddHour and AddMinutes to prevent seconds and miliseconds from showing in the form
                Start = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute),
                End = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute),
                
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddeventViewModel vm)
        {
            bool datesInFuture = true;
            if (vm.End < DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute))
            {
                ModelState.AddModelError("end", "End date must be in the future");
                datesInFuture = false;
            }
            if (vm.Start < DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute))
            {
                ModelState.AddModelError("Start", "End date must be in the future");
                datesInFuture = false;
            }
            if (datesInFuture && vm.End < vm.Start)
            {
                ModelState.AddModelError("end", "End date and time must be later than start date and time");
            }

            string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                Event newEvent = new Event()
                {
                    Name = vm.Name,
                    Start = vm.Start,
                    End = vm.End,
                    Street = vm.Street,
                    Number = vm.Number,
                    PostalCode = vm.PostalCode,
                    Town = vm.Town,
                    Country = vm.SelectedCountry,
                    Description = vm.Description,
                    CategoryId = vm.SelectedCategoryId
                };
                _unitOfWork.EventRepo.Add(newEvent);
                _unitOfWork.Save(); // Save the new event to get the eventId

                if (vm.Thumbnail?.Length > 0 || vm.Thumbnail != null)
                {
                    string path = Path.Combine(this._enviroment.WebRootPath, "images", "Event-thumbnails", $"Event-{newEvent.EventId}");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    string filename = Path.GetFileName(vm.Thumbnail.FileName);
                    using (var stream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    {
                        stream.Position = 0;
                        await vm.Thumbnail.CopyToAsync(stream);
                    }

                    newEvent.Thumbnail = $"Event-thumbnails/Event-{newEvent.EventId}/{filename}";
                }
                else
                {
                    newEvent.Thumbnail = $"Event-thumbnails/Placeholder/Placeholder.png";
                }
                // Update the event to include the img file path
                _unitOfWork.EventRepo.Update(newEvent);
                _unitOfWork.Save();

                // Create the default collaboration for the user who created the event
                _unitOfWork.CollaborationRepo.Add(new Collaboration() { Organizer = true, UserId = userId, EventId = newEvent.EventId });
                _unitOfWork.Save();

                // Send the user to the collaboration / edit action to edit the collaborations of his/hers new event
                return RedirectToAction("Edit", "Collaboration", new { id = newEvent.EventId });
            }

            vm.Categories = new SelectList(_unitOfWork.CategoryRepo.Get(), "CategoryId", "Name");
            vm.Countries = new SelectList(new List<string> { "Belgium", "Netherlands", "Luxembourg" });
            CustomUser user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();
            vm.Collaboration = new Collaboration() // Add a default collaboration for the user that is creating the event
            {
                Organizer = true,
                UserId = user.Id,
                User = user
            };
            return View(vm);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            Event evnt = _unitOfWork.EventRepo.Get(e => e.EventId == id, e => e.Collaborations).FirstOrDefault();
            if (evnt != null)
            {
                // Check if the current user is allowed to delete the event (if the user is a organizer of the event)
                string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool authorisedUser = false;
                foreach (Collaboration collab in evnt.Collaborations)
                {
                    if (collab.Organizer && collab.UserId == userId) authorisedUser = true;
                }
                if (this.User.IsInRole("Admin"))
                {
                    authorisedUser = true;
                }

                if (authorisedUser)
                {
                    string path = Path.Combine(this._enviroment.WebRootPath, "images", "Event-thumbnails", $"Event-{evnt.EventId}");
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true); // Delete img dir
                    }

                    _unitOfWork.EventRepo.Delete(evnt);
                    await _unitOfWork.SaveAsync();

                    return RedirectToAction("Index", "Home");
                }
                else
                    return NotFound();
            }
            else
                return NotFound();
        }

        public IActionResult Edit(int? id)
        {
            Event evnt = _unitOfWork.EventRepo.Get(e => e.EventId == id, e => e.Collaborations).FirstOrDefault();
            if (evnt != null)
            {
                // Check if the current user is allowed to edit the event (if the user is a organizer of the event)
                string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool authorisedUser = false;
                foreach (Collaboration collab in evnt.Collaborations)
                {
                    if (collab.Organizer && collab.UserId == userId) authorisedUser = true;
                }
                if (this.User.IsInRole("Admin"))
                {
                    authorisedUser = true;
                }

                if (authorisedUser)
                {
                    evnt.Collaborations.ForEach(c => c.User = _userManager.Users.Where(u => u.Id == c.UserId).FirstOrDefault());
                    EditEventViewModel vm = new EditEventViewModel()
                    {
                        EventId = evnt.EventId,
                        Name = evnt.Name,
                        SelectedCategoryId = evnt.CategoryId,
                        Street = evnt.Street,
                        Number = evnt.Number,
                        PostalCode = evnt.PostalCode,
                        Town = evnt.Town,
                        SelectedCountry = evnt.Country,
                        Start = evnt.Start,
                        End = evnt.End,
                        Thumbnail = evnt.Thumbnail,
                        Description = evnt.Description,
                        Collaborations = evnt.Collaborations,
                        Categories = new SelectList(_unitOfWork.CategoryRepo.Get(), "CategoryId", "Name"),
                        Countries = new SelectList(new List<string> { "Belgium", "Netherlands", "Luxembourg" })
                    };
                    return View(vm);
                }
                else
                    return NotFound();
            }
            else
                return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditEventViewModel vm)
        {
            if (vm.End < vm.Start)
            {
                ModelState.AddModelError("end", "End date and time must be later than start date and time");
            }

            Event evnt = null;
            if (ModelState.IsValid)
            {
                evnt = new Event() 
                {
                    EventId = vm.EventId,
                    Name = vm.Name,
                    CategoryId = vm.SelectedCategoryId,
                    Street = vm.Street,
                    Number = vm.Number,
                    PostalCode = vm.PostalCode,
                    Town = vm.Town,
                    Country = vm.SelectedCountry,
                    Start = vm.Start,
                    End = vm.End,
                    Thumbnail = vm.Thumbnail,
                    Description = vm.Description,
                    Collaborations = vm.Collaborations
                };

				if (vm.NewThumbnail?.Length > 0 || vm.NewThumbnail != null)
				{
					string path = Path.Combine(this._enviroment.WebRootPath, "images", "Event-thumbnails", $"Event-{vm.EventId}");
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true); // Delete old dir
                    }
                    Directory.CreateDirectory(path);
                    string filename = Path.GetFileName(vm.NewThumbnail.FileName);
					using (var stream = new FileStream(Path.Combine(path, filename), FileMode.Create))
					{
						stream.Position = 0;
						await vm.NewThumbnail.CopyToAsync(stream);
					}
                    evnt.Thumbnail = $"Event-thumbnails/Event-{vm.EventId}/{vm.NewThumbnail.FileName}";
				}

				_unitOfWork.EventRepo.Update(evnt);
                int result = await _unitOfWork.SaveAsync();
                if (result > 0)
                {
                    return RedirectToAction("Index", new { id = evnt.EventId});
                }
            }
            vm.Categories = new SelectList(_unitOfWork.CategoryRepo.Get(), "CategoryId", "Name");
            vm.Countries = new SelectList(new List<string> { "Belgium", "Netherlands", "Luxembourg" });
            evnt = _unitOfWork.EventRepo.Get(e => e.EventId == vm.EventId, e => e.Collaborations).FirstOrDefault();
            evnt.Collaborations.ForEach(c => c.User = _userManager.Users.Where(u => u.Id == c.UserId).FirstOrDefault());
            vm.Collaborations = evnt.Collaborations;
            return View(vm);
        }
    }
}
