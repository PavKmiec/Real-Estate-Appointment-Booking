using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BookingWebsite.Models;
using BookingWebsite.Models.ViewModel;
using BookingWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BookingWebsite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        // role manager
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            // additional property for registration
            [Required]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Street Address")]
            public string StreetAddress { get; set; }

            [Required]
            public string City { get; set; }

            [Required]
            [Display(Name = "Post Code")]
            public string PostCode { get; set; }


            [Required]
            [Display(Name ="Phone Number")]
            public string PhoneNumber { get; set; }

            //[Display(Name = "Super Admin")]
            //public bool IsSuperAdmin { get; set; }

           



        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // get radiobutton value to get role value 
            string role = Request.Form["rdUserRole"].ToString();

            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                // registration model modification to suit our model
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    PhoneNumber = Input.PhoneNumber,
                    StreetAddress = Input.StreetAddress,
                    City = Input.City,
                    PostCode = Input.PostCode,




                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    // create all roles
                    if (!await _roleManager.RoleExistsAsync(SD.SuperAdminEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.SuperAdminEndUser));

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

                    // set roles based on selection, i none selected make a Customer role
                    if (role == SD.AdminEndUser)
                    {
                        await _userManager.AddToRoleAsync(user, SD.AdminEndUser);

                    }
                    else
                    {
                        if (role == SD.Employee)
                        {
                            await _userManager.AddToRoleAsync(user, SD.Employee);

                        }
                        else
                        {
                            if (role == SD.SellerEndUser)
                            {
                                await _userManager.AddToRoleAsync(user, SD.SellerEndUser);
                                

                            }
                            // here we also make sure that only customer user is logged in automatically - to prevent logging-in when admin creates employees 
                            else
                            {
                                await _userManager.AddToRoleAsync(user, SD.CustomerEndUser);
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                return LocalRedirect(returnUrl);



                            }

                        }

                    }


                    return RedirectToAction("Index", "User", new {area = "Admin"});

                    //TODO change this to standard user "customer" after development stage completed


                    //if (!await  _roleManager.RoleExistsAsync(SD.AdminEndUser))
                    //{
                    //    await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));

                    //}
                    //if (!await _roleManager.RoleExistsAsync(SD.SuperAdminEndUser))
                    //{
                    //    await _roleManager.CreateAsync(new IdentityRole(SD.SuperAdminEndUser));

                    //}

                    //if (Input.IsSuperAdmin)
                    //{
                    //    await _userManager.AddToRoleAsync(user, SD.SuperAdminEndUser);
                    //}
                    //else
                    //{
                    //    await _userManager.AddToRoleAsync(user, SD.AdminEndUser);

                    //}


                    _logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


                    //return RedirectToAction("Index", "AdminUsers", new {area = "Admin"}); //TODO check
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
