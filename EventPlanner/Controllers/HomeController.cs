using EventPlanner.Data;
using EventPlanner.Data.UnitOfWork;
using EventPlanner.Models;
using EventPlanner.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EventPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            EventListViewModel vm = new EventListViewModel()
            {
                Categories = new SelectList(_unitOfWork.CategoryRepo.Get(), "CategoryId", "Name"),
                Events = _unitOfWork.EventRepo.Get(e => e.Category).ToList()
            };
            return View(vm);
        }

        public IActionResult Filter(string categoryId, string name)
        {
            EventListViewModel vm = new EventListViewModel()
            {
                Categories = new SelectList(_unitOfWork.CategoryRepo.Get(), "CategoryId", "Name")
            };
            vm.Events = _unitOfWork.EventRepo.Get(e => e.Category).ToList();

            if (!string.IsNullOrEmpty(name))
                vm.Events = vm.Events.Where(e => e.Name.ToLower().Contains(name.ToLower())).ToList();
            if (!string.IsNullOrEmpty(categoryId) && int.TryParse(categoryId, out int categoryIdInt) && categoryId != "0")
                vm.Events = vm.Events.Where(e => e.CategoryId == categoryIdInt).ToList();

            return PartialView("_ShowEventsPartial", vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
