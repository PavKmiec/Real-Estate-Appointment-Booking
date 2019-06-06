using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingWebsite.Data;
using BookingWebsite.Models;
using BookingWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingWebsite.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class AdminUsersController : Controller
    {

        private readonly ApplicationDbContext _db;

        private readonly UserManager<ApplicationUser> _userManager;


        public AdminUsersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;


        }      


        public IActionResult Index()
        {


            return View(_db.ApplicationUser.Include(u => u.Branch).ToList());

            //_db.ApplicationUser.Include(u => u.Branch).ToList()
        }


        // get edit 

        public async Task<IActionResult> Edit(string id)
        {
            if (id== null || id.Trim().Length==0 )
            {
                return NotFound();

            }

            var userFromDb = await _db.ApplicationUser.FindAsync(id);
            if (userFromDb==null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }


        // POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                ApplicationUser userFromDb = _db.ApplicationUser.Where(u => u.Id == id).FirstOrDefault();
                userFromDb.Name = applicationUser.Name;
                userFromDb.PhoneNumber = applicationUser.PhoneNumber;

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }


            // if model not valid - returnt to view passing applicationUser object
            return View(applicationUser);

        }


        // get edit 

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();

            }

            var userFromDb = await _db.ApplicationUser.FindAsync(id);
            if (userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }



        // POST

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(string id)
        {
            

                ApplicationUser userFromDb = _db.ApplicationUser.Where(u => u.Id == id).FirstOrDefault();
            userFromDb.LockoutEnd = DateTime.Now.AddDays(100);
               

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            


            

        }

    }
}