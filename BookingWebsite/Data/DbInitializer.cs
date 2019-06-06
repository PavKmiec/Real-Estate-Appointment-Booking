using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingWebsite.Models;
using BookingWebsite.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingWebsite.Data
{
    public class DbInitializer :IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async void Initialize()
        {
            // check if there are migrations
            if (_db.Database.GetPendingMigrations().Count() > 0)
            {
                _db.Database.Migrate();
            }


            if (_db.Roles.Any(r => r.Name == SD.SuperAdminEndUser)) return;

            _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.SuperAdminEndUser)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                Name = "Admin User",
                EmailConfirmed = true,
                StreetAddress = "1 Main Street",
                PostCode = "G1",
                City = "GlasVegas",
                PhoneNumber = "014111111111"
                
            }, "Pass123@").GetAwaiter().GetResult();

            IdentityUser user = await _db.Users.Where(u => u.Email == "admin@gmail.com").FirstOrDefaultAsync();

            await _userManager.AddToRoleAsync(user, SD.SuperAdminEndUser);



            //var product1 = new Products()
            //{
            //    Name = "3 Bed House, 1 Main Street",
            //    Price = 45000,
            //    Description = "<p><span style=\"color: #595963; font-family: Arial, sans-serif; font-size: 16px; letter-spacing: 0.2px;\"><strong>This exceptional, three bedroom, second floor, luxury apartment has been professionally modernised</strong> " +
            //                  "and carefully refined to create what is without a doubt, the finest apartment currently for sale in Garnethill." +
            //                  "</span><br style=\"box-sizing: inherit; color: #595963; font-family: Arial, sans-serif; font-size: 16px; letter-spacing: 0.2px;\" /><br style=\"box-sizing: inherit; color: #595963; font-family: Arial, sans-serif; font-size: 16px; letter-spacing: 0.2px;\" /><span style=\"color: #595963; font-family: Arial, sans-serif; font-size: 16px; letter-spacing: 0.2px;\">" +
            //                  "As you will see from the attached photographs and HD Video, this beautiful home has been completely upgraded and modernised with great skill and attention to detail. " +
            //                  "The accommodation consists of five primary apartments which are all of superb proportions with stylish, neutral decor and attractive, high quality floor coverings. The spacious kitchen is brand new and contains full compliment of fitted <strong>Bosch appliances</strong>, " +
            //                  "composite granite-style work surfaces and a much desired centre island which can be used as a breakfast bar. There is a stunning main bathroom with four piece suite including stand-alone bath with pillar tap and a carefully designed vanity unit which provides storage space for numerous toiletries. The master bedroom has an en-suite shower room with white, three-piece suite and electric shower.</span></p>",
            //    ProductTypes = _db.ProductTypes.Add()

            //};

            //var restultUser = await _userManager.CreateAsync(userAdmin, "Pass123@");
            //await _userManager.AddToRoleAsync(userAdmin, SD.SuperAdminEndUser);


        }

    }
    }
