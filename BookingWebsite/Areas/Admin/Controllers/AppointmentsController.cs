using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Appointments Controller 
    /// </summary>
    /// Authorization for users specified in SD utility class
    [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {

        // we need db (dependency injection)
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// constructor
        /// </summary>
        public AppointmentsController(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }


        /// <summary>
        /// index method for Appointments controller, using async has many benefits, main being requests can be served even when another request is still processing,
        /// this is combined with using "await"
        /// we also use parameters in this action to enable pseudo-search (filtering) - rather that making additional method for searching, we can use this one
        /// we will receive any parameters if user has entered them
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index(string searchName=null, string searchEmail=null, string searchPhone=null, string searchDate=null)
        {
            // to identify what is the current user we wil use the security claims principal
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;

            // store in var; claims identity = convert to (claimsIdentity) 
            var claimsIdentity = (ClaimsIdentity) this.User.Identity;

            // get claim value - having this we can retrieve used id by claim.value - this is easier in .net but in .net core this is the way to do this
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // new VM, with list of appointments - initialization
            AppointmentViewModel appointmentVM = new AppointmentViewModel()
            {
                Appointments = new List<Appointments>()
            };


            

            appointmentVM.Appointments = _db.Appointments.Include(a => a.SalesPerson).ToList();

            // check if the role of logged in user is admin or super admin (Admin = Sales Person)
            // if Sales person then only their own appointments are visible to them
            if (User.IsInRole(SD.AdminEndUser))
            {
                // this is where we use the above created claim to retrieve user ID // 
                appointmentVM.Appointments =
                    appointmentVM.Appointments.Where(a => a.SalesPersonId == claim.Value).ToList();
            }


            // filter criteria
            if (searchName != null)
            {
                // checking if customer name from appointment View Model matches to the passes string that user has entered- to list in case more than one match
                appointmentVM.Appointments = appointmentVM.Appointments
                    .Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();

            }


            // filter criteria Email
            if (searchEmail != null)
            {
                // checking if customer email from appointment View Model matches to the passed string that user has entered - to list in case more than one match
                appointmentVM.Appointments = appointmentVM.Appointments
                    .Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();

            }


            // filter criteria phone number
            if (searchPhone != null)
            {
                // checking if customer phone from appointment View Model matches to the passes string that user has entered- to list in case more than one match
                appointmentVM.Appointments = appointmentVM.Appointments
                    .Where(a => a.CustomerPhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToList();

            }

            // filter criteria date
            if (searchDate != null)
            {
                // checking if date from appointment View Model matches to the passes string that user has entered- to list in case more than one match

                // because we receive a string: try to convert to datetime
                try
                {
                    // convert to dateTime
                    DateTime appDate = Convert.ToDateTime(searchDate);
                    appointmentVM.Appointments = appointmentVM.Appointments
                        .Where(a => a.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();

                }
                catch (Exception ex)
                {
                    
                }
                

            }


            // return a View with a view model that we created
            return View(appointmentVM);
        }
    }
}