using EventPlanner.Areas.Identity.Data;
using EventPlanner.Data.UnitOfWork;
using EventPlanner.Models;
using EventPlanner.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventPlanner.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<CustomUser> _userManager;

        public ReviewController(IUnitOfWork unitOfWork, UserManager<CustomUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Create(int? eventId, string userId)
        {
            if (eventId != null && !string.IsNullOrEmpty(userId))
            {
                if (userId == User.FindFirstValue(ClaimTypes.NameIdentifier)) // Check that the userId matches the current users id
                {
                    CreateReviewViewModel vm = new CreateReviewViewModel()
                    {
                        Event = _unitOfWork.EventRepo.Get(e => e.EventId == eventId).FirstOrDefault(),
                        EventId = (int)eventId,
                        CurrentUserId = userId
                    };
                    return View(vm);
                }
                return RedirectToAction("Index", "Event", new { id = eventId });
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReviewViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.EventId != null && !string.IsNullOrEmpty(vm.CurrentUserId))
                {
                    if (vm.CurrentUserId == User.FindFirstValue(ClaimTypes.NameIdentifier)) // Check that the userId matches the current users id
                    {
                        _unitOfWork.ReviewRepo.Add(new Review()
                        {
                            EventId= vm.EventId,
                            UserId = vm.CurrentUserId,
                            Title = vm.Title,
                            Description = vm.Description,
                            Rating = (int)vm.Rating
                        });
                        await _unitOfWork.SaveAsync();
                    }
                    return RedirectToAction("Index", "Event", new { id = vm.EventId });
                }
            }
            vm.Event = _unitOfWork.EventRepo.Get(e => e.EventId == vm.EventId).FirstOrDefault();
            return View(vm);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            Review review = _unitOfWork.ReviewRepo.Get(r => r.ReviewId == id).FirstOrDefault();
            if (review != null)
            {
                _unitOfWork.ReviewRepo.Delete(review);
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Index", "Event", new { id = review.EventId });
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
