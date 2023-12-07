﻿#nullable disable
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
using Microsoft.AspNetCore.Authorization;

//Generated from Custom Template.
namespace MVC.Controllers
{
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        // TODO: Add service injections here
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: Roles
        public IActionResult Index()
        {
            List<RoleModel> roleList = _roleService.Query().ToList();
            return View(roleList);
        }

        // GET: Roles/Details/5
        public IActionResult Details(int id)
        {
            RoleModel role = _roleService.Query().SingleOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            // TODO: Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RoleModel role)
        {
            if (ModelState.IsValid)
            {
                var result = _roleService.Add(role);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // we must put TempData["Message"] in the Index view
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(role);
        }

        // GET: Roles/Edit/5
        public IActionResult Edit(int id)
        {
            RoleModel role = _roleService.Query().SingleOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound();
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(role);
        }

        // POST: Roles/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RoleModel role)
        {

            if (ModelState.IsValid)
            {
                var result = _roleService.Update(role);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // we must put TempData["Message"] in the Index view
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            return View(role);
        }

        public IActionResult Delete(int id)
        {
            var result = _roleService.Delete(id);
            TempData["Message"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
	}
}
