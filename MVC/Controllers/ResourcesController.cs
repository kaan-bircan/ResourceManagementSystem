#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess.Contexts;
using DataAccess.Entities;
using Business.Services;
using Business.Models;
using Business;
using System.Diagnostics;
using Business.Results.Bases;
using Microsoft.AspNetCore.Authorization;


//Generated from Custom Template.
namespace MVC.Controllers
{
    [Authorize]
    public class ResourcesController : Controller
    {
        // TODO: Add service injections here
        private readonly IResourceService _resourceService;
        private readonly IUserService _userService;

        public ResourcesController(IResourceService resourceService, IUserService userService)
        {
            _resourceService = resourceService;
            _userService = userService;
        }

        [AllowAnonymous]
        // GET: Resources
        public IActionResult Index()
        {
            List<ResourceModel> resourceList = _resourceService.GetList();
            return View(resourceList);
        }

        // GET: Resources/Details/5
        public IActionResult Details(int id)
        {
            ResourceModel resource = _resourceService.GetItem(id); // TODO: Add get item service logic here
            if (resource == null)
            {
                return View("_Error", "Resource not found!!");
            }
            return View(resource);
        }

        // GET: Resources/Create
        public IActionResult Create()
        {
            ViewBag.UserId = new MultiSelectList(_userService.Query().ToList(), "Id", "UserName");
            return View();
        }

        // POST: Resources/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ResourceModel resource)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add insert service logic here
                Result result = _resourceService.Add(resource);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;  
                    return RedirectToAction(nameof(Index));

                }
                ModelState.AddModelError("",result.Message);
            }
			ViewBag.UserId = new MultiSelectList(_userService.Query().ToList(), "Id", "UserName");// TODO: Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
			return View(resource);
        }

        // GET: Resources/Edit/5
        public IActionResult Edit(int id)
        {
            ResourceModel resource = _resourceService.GetItem(id); // TODO: Add get item service logic here
            if (resource == null)
            {
                return View("_Error", "Resource not found!");
            }
			ViewBag.UserId = new MultiSelectList(_userService.Query().ToList(), "Id", "UserName");// TODO: Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
			return View(resource);
        }

        // POST: Resources/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ResourceModel resource)
        {
            if (ModelState.IsValid)
            {
                var result = _resourceService.Update(resource);
				if (result.IsSuccessful)
				{
					TempData["Message"] = result.Message; // we must put TempData["Message"] in the Index view
					return RedirectToAction(nameof(Details), new {id = resource.Id});
				}
				ModelState.AddModelError("", result.Message);
			}
			ViewBag.UserId = new MultiSelectList(_userService.Query().ToList(), "Id", "UserName");// TODO: Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
			return View(resource);
        }

        // GET: Resources/Delete/5
        public IActionResult Delete(int id)
        {
            var result = _resourceService.Delete(id); // TODO: Add get item service logic here
            TempData["Message"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        
	}
}
