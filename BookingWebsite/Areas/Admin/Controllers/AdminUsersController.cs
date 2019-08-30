using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingWebsite.Data;
using BookingWebsite.Models;
using BookingWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingWebsite.Areas.Admin.Controllers
{

    /// <summary>
    /// Administration of all users controller CRUD
    /// </summary>
    [Authorize(Roles = SD.SuperAdminEndUser + "," + SD.AdminEndUser)]
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        // dependency injection - we need db
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="db"></param>
        public AdminUsersController(ApplicationDbContext db)
        {
            _db = db;


        }      

        /// <summary>
        ///  displau list of users
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View(_db.ApplicationUser.Include(u=>u.Branch).ToList());
        }


       

        /// <summary>
        /// GET Edit action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Edit POST action
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationUser"></param>
        /// <returns></returns>
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


            // if model not valid - return to view passing applicationUser object
            return View(applicationUser);

        }


        
        /// <summary>
        /// Delete Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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



        
        /// <summary>
        /// Delete POST action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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