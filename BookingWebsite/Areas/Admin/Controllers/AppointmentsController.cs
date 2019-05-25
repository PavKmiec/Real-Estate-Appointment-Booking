using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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

        //define page size - number per page
        private int PageSize = 3; //TODO change this after seed to larger number

        /// <summary>
        /// constructor
        /// </summary>
        public AppointmentsController(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }




        //GET Edit action method
        public async Task<IActionResult> Edit(int? id)
        {

            //check if id is null
            if (id == null)
            {
                return NotFound();
            }

            // we need to retrieve all of the products for an appointment, so we need to combine from ProductsSelectedForAppointment and Products models
            // and filter that based on appointment ID that is passed in and retrieve those products
            // we will use Linq to do that
            // join on tables, converted selected products to IEnumerable of products
            var productList = (IEnumerable<Products>) (from p in _db.Products
                join a in _db.ProductsSelectedForAppointments on p.Id equals a.ProductId
                where a.AppointmentId == id
                select p).Include("ProductTypes");


            // We need new AppointmentsViewModel and to populate with specific appointment data 
            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {

                Appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationUser.ToList(),
                Products = productList.ToList()

            };
            
            
            // now that we have our products we ween to pas the m on to the view don't we? ;-) 

            return View(objAppointmentVM);
        }



        [HttpPost]
        public async Task<IActionResult> Edit(int id, AppointmentDetailsViewModel objAppointmentVM)
        {


            // chack id Id is valid
            if (id != objAppointmentVM.Appointment.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {

                // combine appointment time and date
                objAppointmentVM.Appointment.AppointmentDate = objAppointmentVM.Appointment.AppointmentDate
                    .AddHours(objAppointmentVM.Appointment.AppointmentTime.Hour)
                    .AddMinutes(objAppointmentVM.Appointment.AppointmentTime.Minute);


                // retrive appointment object from DB //TODO see if can single call to FirstOrDefault()
                var appointmentFromDb = _db.Appointments.Where(a => a.Id == objAppointmentVM.Appointment.Id).FirstOrDefault();

                // upadte fields taken from the view
                appointmentFromDb.CustomerName = objAppointmentVM.Appointment.CustomerName;
                appointmentFromDb.CustomerEmail = objAppointmentVM.Appointment.CustomerEmail;
                appointmentFromDb.CustomerPhoneNumber = objAppointmentVM.Appointment.CustomerPhoneNumber;
                appointmentFromDb.AppointmentDate = objAppointmentVM.Appointment.AppointmentDate;
                appointmentFromDb.isConfirmed = objAppointmentVM.Appointment.isConfirmed;

                // check if admin to enable Sales  Person change / assign
                if (User.IsInRole(SD.SuperAdminEndUser))
                {
                    // update Sales Person
                    appointmentFromDb.SalesPersonId = objAppointmentVM.Appointment.SalesPersonId;

                }
                // save
                await _db.SaveChangesAsync();



                // and redirect
                return RedirectToAction(nameof(Index));


            }
            // if state not valid return
            
            return View(objAppointmentVM);



        }


        /// <summary>
        /// index method for Appointments controller, using async has many benefits, main being requests can be served even when another request is still processing,
        /// this is combined with using "await"
        /// we also use parameters in this action to enable pseudo-search (filtering) - rather that making additional method for searching, we can use this one
        /// we will receive any parameters if user has entered them
        /// productPage set 1 as default: if not parameter is passed it will load first page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index(int productPage=1, string searchName=null, string searchEmail=null, string searchPhone=null, string searchDate=null)
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


            // url creation

            StringBuilder param = new StringBuilder();
            // building based on parameters that are received - making sure that this also works with search parameters
            // because number of paginated pages may vary based on the search criteria
            // we can use ":" fro page number because we have set that in PageLinkTagHelper function 

            // appending parameters tu URL that may be passed in
            param.Append("/Admin/Appointments?productPage=:");

            // check
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            param.Append("&searchDate=");
            if (searchDate != null)
            {
                param.Append(searchDate);
            }






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

            // count how many appointmats there is after the search criteria

            var count = appointmentVM.Appointments.Count(); //TODO check this

            // order and filter
            // skipping the appointments that were displayed on previous page
            // for example if we are on page two this makes sure that appointments listed on page one are not displayed on page one
            // in short - it feaches the correct records for the page we are on
            appointmentVM.Appointments = appointmentVM.Appointments.OrderBy(p => p.AppointmentDate)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize).ToList();
            
            // now we need to populate PagingInfoModel

            appointmentVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };


            
            // return a View with a view model that we created
            return View(appointmentVM);
        }



        //GET Details action method
        public async Task<IActionResult> Details(int? id)
        {

            //check if id is null
            if (id == null)
            {
                return NotFound();
            }

            // we need to retrieve all of the products for an appointment, so we need to combine from ProductsSelectedForAppointment and Products models
            // and filter that based on appointment ID that is passed in and retrieve those products
            // we will use Linq to do that
            // join on tables, converted selected products to IEnumerable of products
            var productList = (IEnumerable<Products>)(from p in _db.Products
                join a in _db.ProductsSelectedForAppointments on p.Id equals a.ProductId
                where a.AppointmentId == id
                select p).Include("ProductTypes");


            // We need new AppointmentsViewModel and to populate with specific appointment data 
            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {

                Appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationUser.ToList(),
                Products = productList.ToList()

            };


            // now that we have our products we ween to pas the m on to the view don't we? ;-) 

            return View(objAppointmentVM);
        }




        //GET Delete action method
        public async Task<IActionResult> Delete(int? id)
        {

            //check if id is null
            if (id == null)
            {
                return NotFound();
            }

            // we need to retrieve all of the products for an appointment, so we need to combine from ProductsSelectedForAppointment and Products models
            // and filter that based on appointment ID that is passed in and retrieve those products
            // we will use Linq to do that
            // join on tables, converted selected products to IEnumerable of products
            var productList = (IEnumerable<Products>)(from p in _db.Products
                join a in _db.ProductsSelectedForAppointments on p.Id equals a.ProductId
                where a.AppointmentId == id
                select p).Include("ProductTypes");


            // We need new AppointmentsViewModel and to populate with specific appointment data 
            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {

                Appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationUser.ToList(),
                Products = productList.ToList()

            };


            // now that we have our products we ween to pas the m on to the view don't we? ;-) 

            return View(objAppointmentVM);
        }



        /// <summary>
        /// POST Delete Action Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DtleteConfirmed(int id)
        {

            // we need appointments from db
            var appointment = await _db.Appointments.FindAsync(id);
            _db.Appointments.Remove(appointment);
            await _db.SaveChangesAsync();
            TempData.Add("Delete"," You have successfully removed an appointment,... now what? ;-)");
            return RedirectToAction(nameof(Index));
        }
    }
}