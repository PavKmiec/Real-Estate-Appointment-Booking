using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookingWebsite.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            return View(await _db.ApplicationUser.Where(u=>u.Id !=claim.Value).ToListAsync());
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

    }
}