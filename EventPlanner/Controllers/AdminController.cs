using EventPlanner.Areas.Identity.Data;
using EventPlanner.Data.UnitOfWork;
using EventPlanner.Models;
using EventPlanner.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventPlanner.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<CustomUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AdminController(IUnitOfWork unitOfWork, UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult GrantPremissions()
        {
            GrantPremissionsViewModel vm = new GrantPremissionsViewModel()
            {
                Users = new SelectList(_userManager.Users.ToList(), "Id", "UserName"),
                Roles = new SelectList(_roleManager.Roles.ToList(), "Id", "Name")
            };
            return View(vm);
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> GrantPremissions(GrantPremissionsViewModel vm)
        {
            if (ModelState.IsValid) 
            {
                CustomUser user = await _userManager.FindByIdAsync(vm.UserId);
                IdentityRole role = await _roleManager.FindByIdAsync(vm.RoleId);


                if (user != null && role != null)
                {
                    IdentityResult result = await _userManager.AddToRoleAsync(user, role.Name);
                    if (result.Succeeded)
                        return RedirectToAction("Users");
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
                else
                    ModelState.AddModelError("", "User or role not found");
            }
            vm.Users = new SelectList(_userManager.Users.ToList(), "Id", "UserName");
            vm.Roles = new SelectList(_roleManager.Roles.ToList(), "Id", "Name");
            return View(vm);
        }

        #region Users
        public IActionResult Users()
        {
            UsersListViewModel vm = new UsersListViewModel()
            {
                Users = _userManager.Users.ToList()
            };

            return View(vm);
        }
        public IActionResult UserDetail(string id)
        {
            CustomUser user = _userManager.Users.Where(u => u.Id.Equals(id)).FirstOrDefault();
            if (user != null)
            {
                UserDetailViewModel vm = new UserDetailViewModel()
                {
                    Name = user.Name,
                    FirstName = user.FirstName,
                    BirthDate = user.Birthdate,
                    Gender = user.Gender.ToString(),
                    Email = user.Email
                };
                return View(vm);
            }
            else
            {
                UsersListViewModel vm = new UsersListViewModel()
                {
                    Users = _userManager.Users.ToList()
                };
                return View(nameof(Users), vm);
            }
        }
        public async Task<IActionResult> DeleteUser(string id)
        {
            CustomUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction(nameof(Users));
                else
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
            }
            else
                ModelState.AddModelError("", "User not found");
            return View(nameof(Users), new UsersListViewModel()
            {
                Users = _userManager.Users.ToList()
            });
        }
        #endregion

        #region Events
        public IActionResult Events()
        {
            EventsListViewModel vm = new EventsListViewModel()
            {
                Events = _unitOfWork.EventRepo.Get(e => e.Category).ToList()
            };

            return View(vm);
        }
        public IActionResult EventDetail(int id)
        {
            Event e = _unitOfWork.EventRepo.Get(e => e.EventId.Equals(id), e => e.Category, e => e.Reviews).FirstOrDefault();
            if (e != null)
            {
                AdminEventDetailViewModel vm = new AdminEventDetailViewModel()
                {
                    Name = e.Name,
                    Start = e.Start,
                    End = e.End,
                    Street = e.Street,
                    Number = e.Number,
                    PostalCode = e.PostalCode,
                    Town = e.Town,
                    Country = e.Country,
                    Category = e.Category,
                    Description = e.Description,
                    Thumbnail = e.Thumbnail,
                    Reviews = e.Reviews
                };
                return View(vm);
            }
            else
            {
                EventsListViewModel vm = new EventsListViewModel()
                {
                    Events = _unitOfWork.EventRepo.Get(e => e.Category).ToList()
                };
                return View(nameof(Events), vm);
            }
        }
        public IActionResult DeleteEvent(int id)
        {
            Event e = _unitOfWork.EventRepo.Get(e => e.EventId.Equals(id)).FirstOrDefault();
            if (e != null)
            {
                _unitOfWork.EventRepo.Delete(e);
                if (_unitOfWork.Save() > 0)
                    return RedirectToAction(nameof(Events));
                else
                    ModelState.AddModelError("", "Event not deleted");
            }
            else
                ModelState.AddModelError("", "Event not found");
            return View(nameof(Events), new EventsListViewModel()
            {
                Events = _unitOfWork.EventRepo.Get(e => e.Category).ToList()
            });
        }

        #region Review
        public IActionResult ReviewDetail(int id)
        {
            Review review = _unitOfWork.ReviewRepo.Get(r => r.ReviewId.Equals(id), r => r.User, r => r.Event).FirstOrDefault();
            if (review != null)
            {
                ReviewDetailViewModel vm = new ReviewDetailViewModel()
                {
                    Title = review.Title,
                    Description = review.Description,
                    Date = review.Date,
                    Event= review.Event,
                    User = review.User,
                    Rating = review.Rating
                };
                return View(vm);
            }
            else
            {
                EventsListViewModel vm = new EventsListViewModel()
                {
                    Events = _unitOfWork.EventRepo.Get(e => e.Category).ToList()
                };
                return View(nameof(Events), vm);
            }
        }
        public IActionResult DeleteReview(int id)
        {
            Review review = _unitOfWork.ReviewRepo.Get(r => r.ReviewId.Equals(id)).FirstOrDefault();
            if (review != null)
            {
                _unitOfWork.ReviewRepo.Delete(review);
                if (_unitOfWork.Save() > 0)
                    return RedirectToAction(nameof(EventDetail), new { id = review.EventId });
                else
                    ModelState.AddModelError("", "Review not deleted");
            }
            else
                ModelState.AddModelError("", "Review not found");
            return View(nameof(Events), new EventsListViewModel()
            {
                Events = _unitOfWork.EventRepo.Get(e => e.Category).ToList()
            });
        }
        #endregion
        #endregion
    }
}
