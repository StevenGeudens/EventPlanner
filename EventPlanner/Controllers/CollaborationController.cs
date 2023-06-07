using EventPlanner.Areas.Identity.Data;
using EventPlanner.Data.UnitOfWork;
using EventPlanner.Models;
using EventPlanner.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventPlanner.Controllers
{
    [Authorize]
    public class CollaborationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<CustomUser> _userManager;

        public CollaborationController(IUnitOfWork unitOfWork, UserManager<CustomUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Edit(int? id)
        {
            Event evnt = _unitOfWork.EventRepo.Get(e => e.EventId == id).FirstOrDefault();
            if (evnt != null) 
            {
                string userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                List<Collaboration> collaborations = _unitOfWork.CollaborationRepo.Get(c => c.EventId == evnt.EventId).ToList();
                // Check if the currently logged in user is a organizer of the event and has the right to edit the collaborations
                bool authorisedUser = false;
                foreach (Collaboration collab in collaborations)
                {
                    if (collab.Organizer && collab.UserId == userId) authorisedUser = true;
                }
                if (this.User.IsInRole("Admin"))
                {
                    authorisedUser = true;
                }

                if (authorisedUser)
                {
                    List<string> addedUserIds = new List<string>();
                    collaborations.ForEach(c => addedUserIds.Add(c.UserId));
                    EditCollaboratorsViewModel vm = new EditCollaboratorsViewModel()
                    {
                        Event = evnt,
                        Users = new SelectList(_userManager.Users.Where(u => !addedUserIds.Contains(u.Id)), "Id", "Email"),
                        Collaborations = _unitOfWork.CollaborationRepo.Get(c => addedUserIds.Contains(c.UserId) && c.EventId == id, c => c.User).ToList(),
                        CurrentUserId = userId,
                        EventId = evnt.EventId
                    };
                    return View(vm);
                }
                return NotFound();
            }
            else
                ModelState.AddModelError("", "Event not found");
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCollaboratorsViewModel vm)
        {
            if (vm.SelectedUserId != null)
            {
                _unitOfWork.CollaborationRepo.Add(new Collaboration
                {
                    Organizer = vm.Organizer,
                    EventId = (int)vm.EventId,
                    UserId = vm.SelectedUserId
                });
                await _unitOfWork.SaveAsync();
            }
            return RedirectToAction("Edit", new { id = vm.EventId });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            Collaboration collaboration = _unitOfWork.CollaborationRepo.Get(c => c.CollaborationId == id).FirstOrDefault();
            if (collaboration != null)
            {
                _unitOfWork.CollaborationRepo.Delete(collaboration);
                if (await _unitOfWork.SaveAsync() > 0)
                    return RedirectToAction("Edit", new { id = collaboration.EventId });
                else
                    ModelState.AddModelError("", "Collaboration not deleted");
            }
            else
                ModelState.AddModelError("", "Collaboration not found");
            return NotFound();
        }
    }
}
