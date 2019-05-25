using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookingWebsite.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingWebsite.ViewComponents
{
    /// <summary>
    /// This is anew concept in .net core
    /// and it has similar purpose to partial views
    /// we will use this to display user Name instead of their email address once they log in
    /// </summary>
    public class UserNameViewComponent : ViewComponent
    {
        // we need bd context - based on user id we will retrieve their name from db

        private readonly ApplicationDbContext _db;

        public UserNameViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }


        /// <summary>
        /// getting user from ClaimsIdentity 
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;

            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userFromDb = await _db.ApplicationUser.Where(u => u.Id == claims.Value).FirstOrDefaultAsync();
            return View(userFromDb);

        }
    }
}
