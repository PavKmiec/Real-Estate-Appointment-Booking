using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookingWebsite.Data;
using BookingWebsite.Models;
using BookingWebsite.Models.ViewModel;
using BookingWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.SuperAdminEndUser)]
    public class UserController : Controller
    {

        // dependency injection - we need access to database

        private readonly ApplicationDbContext _db;

        // constructor database object
        public UserController(ApplicationDbContext db)
        {
            _db = db;

        }

        // we need to display all of the users except the one tha is logged in 
        public async Task<IActionResult> Index()
        {
            // in core getting use is done via ClaimsIdentity which is different to standard .net
            var claimsIdentity = (ClaimsIdentity) this.User.Identity;
            // if this claim is null it means tha user is not logged in 
            // and if it is not null the user ID of logged in user will be in "claim.Value" 
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            // excluding logged in user
            var users = new List<ApplicationUser>();


            
            users.AddRange((from u in _db.ApplicationUser
                join ur in _db.UserRoles on u.Id equals ur.UserId
                join r in _db.Roles on ur.RoleId equals r.Id
                where r.Name.Equals(SD.Employee)
                select new ApplicationUser
                {
                    Name = u.Name,
                    Id = u.Id,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    City = u.City,


                }).ToList());

            




            //await _db.ApplicationUser.Where(u=>u.Id !=claim.Value).ToListAsync()
            return View(users.ToList());
        }



        public async Task<IActionResult> Delete(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (applicationUser==null)
            {
                return NotFound();
            }

            _db.Remove(applicationUser);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));



        }

        /// <summary>
        /// Lock user account 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // get application user
            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            // set lockout end to 25 years
            applicationUser.LockoutEnd = DateTime.Now.AddYears(25);

            // save changes to DB
            await _db.SaveChangesAsync();

            // redirect to index
            return RedirectToAction(nameof(Index));
        }



        /// <summary>
        /// Unlock user account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> UnLock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // get application user
            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            // set lockout end to now
            applicationUser.LockoutEnd = DateTime.Now;

            // save changes to DB
            await _db.SaveChangesAsync();

            // redirect to index
            return RedirectToAction(nameof(Index));
        }

    }
}