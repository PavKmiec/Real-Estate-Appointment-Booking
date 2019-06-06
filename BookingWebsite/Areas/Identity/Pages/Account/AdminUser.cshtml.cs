using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingWebsite.Models;
using BookingWebsite.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BookingWebsite.Areas.Identity.Pages.Account
{
    public class AdminUserModel : PageModel
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminUserModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }



        public async  Task<IActionResult> OnGet()
        {
            // create roles for web App and for Super Admin user


            // create all roles
            if (!await _roleManager.RoleExistsAsync(SD.SuperAdminEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.SuperAdminEndUser));


                var userAdmin = new ApplicationUser
                {
                    UserName = "Admin@gmail.com",
                    Email = "Admin@gmail.com",
                    PhoneNumber = "0141000000",
                    Name = "John Doe"
                };

                var resultUser = await _userManager.CreateAsync(userAdmin, "Pass123@");
                await _userManager.AddToRoleAsync(userAdmin, SD.SuperAdminEndUser);

            }

            if (!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));

            }

            if (!await _roleManager.RoleExistsAsync(SD.Employee))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Employee));

            }
            if (!await _roleManager.RoleExistsAsync(SD.CustomerEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));

            }
            if (!await _roleManager.RoleExistsAsync(SD.SellerEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.SellerEndUser));

            }

            return Page();

        }
    }
}