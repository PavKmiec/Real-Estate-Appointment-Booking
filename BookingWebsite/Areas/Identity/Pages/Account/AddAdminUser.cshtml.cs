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
    public class AddAdminUserModel : PageModel
    {

        private readonly UserManager<IdentityUser> _userManager;

        // role manager
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddAdminUserModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnGet()
        {


            // create roles and admin
            // create all roles
            if (!await _roleManager.RoleExistsAsync(SD.SuperAdminEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.SuperAdminEndUser));

                var userAdmin = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    PhoneNumber = "01410000000",
                    Name = "Super Admin",
                    StreetAddress = "11 Main Street",
                    PostCode = "G1",
                    City = "Glasgow"

                };

                var restultUser = await _userManager.CreateAsync(userAdmin, "Pass123@");
                await _userManager.AddToRoleAsync(userAdmin, SD.SuperAdminEndUser);



            }

            if (!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));

                var userManager = new ApplicationUser
                {
                    UserName = "manager@gmail.com",
                    Email = "manager@gmail.com",
                    PhoneNumber = "014111111111",
                    Name = "John Doe",
                    StreetAddress = "12 Main Street",
                    PostCode = "G1",
                    City = "Glasgow"

                };

                var restultUser = await _userManager.CreateAsync(userManager, "Pass123@");
                await _userManager.AddToRoleAsync(userManager, SD.AdminEndUser);


            }

            if (!await _roleManager.RoleExistsAsync(SD.Employee))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Employee));

                var userEmployee = new ApplicationUser
                {
                    UserName = "enployee@gmail.com",
                    Email = "enployee@gmail.com",
                    PhoneNumber = "014222222222",
                    Name = "Amy McDonald",
                    StreetAddress = "13 Main Street",
                    PostCode = "G1",
                    City = "Glasgow"

                };

                var restultUser = await _userManager.CreateAsync(userEmployee, "Pass123@");
                await _userManager.AddToRoleAsync(userEmployee, SD.Employee);



            }
            if (!await _roleManager.RoleExistsAsync(SD.CustomerEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));

            }
            if (!await _roleManager.RoleExistsAsync(SD.SellerEndUser))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.SellerEndUser));

                var userSeller = new ApplicationUser
                {
                    UserName = "seller@gmail.com",
                    Email = "seller@gmail.com",
                    PhoneNumber = "0143333333",
                    Name = "Mike Gunn",
                    StreetAddress = "14 Main Street",
                    PostCode = "G1",
                    City = "Glasgow"

                };

                var restultUser = await _userManager.CreateAsync(userSeller, "Pass123@");
                await _userManager.AddToRoleAsync(userSeller, SD.SellerEndUser);

            }


            


            return Page();

        }
    }
}