using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BookingWebsite.Data;
using BookingWebsite.Models;
using BookingWebsite.Models.ViewModel;
using BookingWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Remotion.Linq.Clauses.ResultOperators;

namespace BookingWebsite.Areas.Admin.Controllers
{
    /// <summary>
    /// Appointments Controller 
    /// </summary>
    /// Authorization for users specified in SD utility class
    [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser + "," + SD.Employee + "," + SD.CustomerEndUser + "," + SD.SellerEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {

        // we need db (dependency injection)
        private readonly ApplicationDbContext _db;

        //// TODO userManager that hopefully get me user email for filtering customer appointments, lets see if it works

        private readonly UserManager<IdentityUser> _userManager;

        //define page size - number per page
        private int PageSize = 6; //TODO change this after seed to larger number

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="userManager"></param>
        public AppointmentsController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _db = dbContext;
            _userManager = userManager;
        }




        /// <summary>
        /// GET Edit Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
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



            // we need to fetch only employees in order to make sure that when manager or admin assigns staff to appointments the list only shows employee users
            // oh well, this smells like a join query... 

            // first thing first, list application users instance
            //var users = new List<ApplicationUser>();



            // join query - to make sure we load only employee users
            //users.AddRange((from user in _db.ApplicationUser
            //    join userRole in _db.UserRoles on user.Id equals userRole.UserId
            //    join role in _db.Roles on userRole.RoleId equals role.Id
            //    where role.Name.Equals(SD.Employee)
            //    select new ApplicationUser
            //    {
            //        Name = user.Name,
            //        Id = user.Id,
            //        Email = user.Email,
            //        PhoneNumber = user.PhoneNumber,
            //        City = user.City,
            //        StreetAddress = user.StreetAddress,
            //        PostCode = user.PostCode

            //    }).ToList());

            var appointment = _db.Appointments.Find(id);

            var employees = GetAvailableEmployees(appointment, id);


            // We need new AppointmentsViewModel and to populate with specific appointment data 
            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {

                Appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                // this is where we use out filtered list of users from the join query above
                SalesPerson = employees.ToList(),
                Products = productList.ToList()

            };


            // now that we have our products we ween to pas the m on to the view don't we? ;-) 

            return View(objAppointmentVM);
        }


        public List<ApplicationUser> GetAvailableEmployees(Appointments appointment, int? id)
        {
            int timeCheck = 0;
            int fullCheck = 0;
            int hoursInAppoint = 0;
            var users = new List<ApplicationUser>();
            var employees = new List<ApplicationUser>();

            //Get all employee users from database.
            users.AddRange((from user in _db.ApplicationUser
                join userRole in _db.UserRoles on user.Id equals userRole.UserId
                join role in _db.Roles on userRole.RoleId equals role.Id
                where role.Name.Equals(SD.Employee)
                select new ApplicationUser
                {
                    Name = user.Name,
                    Id = user.Id,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    City = user.City,
                    StreetAddress = user.StreetAddress,
                    PostCode = user.PostCode
                }).ToList());

            //Get all products associated with Appointment.
            var productList = (IEnumerable<Products>) (from p in _db.Products
                join a in _db.ProductsSelectedForAppointments on p.Id equals a.ProductId
                where a.AppointmentId == id
                select p).Include("ProductTypes");

            //Calculate total time requirement, in hours.
            // based on number of fetched products associated with appointment
            int time = productList.Count() * 2;

            // appointment time
            var appointmentStart = appointment.AppointmentDate.Hour;
            var appointmentEnd = appointment.AppointmentDate.AddHours(time).Hour;

            //Loop for all employees in users list.
            foreach (ApplicationUser au in users)
            {


                // match user id
                if (appointment.SalesPersonId == au.Id)
                {
                    //add
                    employees.Add(au);
                    continue;
                }

                //
                var appointmentsFromDb = _db.Appointments.Where(a => a.SalesPersonId == au.Id &&
                                                                     a.AppointmentDate.Day ==
                                                                     appointment.AppointmentDate.Day &&
                                                                     a.AppointmentDate.Month ==
                                                                     appointment.AppointmentDate.Month &&
                                                                     a.AppointmentDate.Year ==
                                                                     appointment.AppointmentDate.Year);

                if (appointmentsFromDb.Count().Equals(0))
                {
                    employees.Add(au);
                }
                else
                {
                    foreach (Appointments appoint in appointmentsFromDb)
                    {
                        var associatedProducts = (IEnumerable<Products>) (from p in _db.Products
                            join a in _db.ProductsSelectedForAppointments on p.Id equals a.ProductId
                            where a.AppointmentId == id
                            select p).Include("ProductTypes");


                        // if there there are products times that 2 (2hours each appointment
                        if (!associatedProducts.Count().Equals(0))
                            hoursInAppoint = associatedProducts.Count() * 2;

                        //Ignore for the moment
                        //get appointment hour
                        var appointStart = appoint.AppointmentDate.Hour;
                        // get appointment end time by adding appointment hours- calculated above
                        var appointEnd = appoint.AppointmentDate.AddHours(hoursInAppoint).Hour;


                        if (appointmentStart == appointEnd)
                        {
                            timeCheck++;
                        }

                        if (appointmentEnd == appointmentStart)
                        {
                            timeCheck++;
                        }

                        for (int i = 0; i <= time - 1; i++)
                        {
                            if ((appointmentStart + i) < appointStart || (appointmentStart + i) > appointEnd)
                            {
                                timeCheck++;
                            }
                        }

                        if (timeCheck == time)
                        {
                            fullCheck++;
                        }
                        else
                            break;

                        if (fullCheck == appointmentsFromDb.Count())
                        {
                            employees.Add(au);
                            fullCheck = 0;
                        }

                        timeCheck = 0;
                    }

                    fullCheck = 0;
                    timeCheck = 0;
                }
            }

            return employees;
        }

        /// <summary>
        /// Edit POST action
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objAppointmentVM"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
        public async Task<IActionResult> Edit(int id, AppointmentDetailsViewModel objAppointmentVM)
        {
            int available = 0;

            var appointment = _db.Appointments.Find(id);

            var employees = GetAvailableEmployees(appointment, id);

            foreach (ApplicationUser u in employees)
            {
                if (objAppointmentVM.Appointment.SalesPersonId == u.Id)
                {
                    available = 1;
                }

            }

            if (available == 0)
            {
                return View(objAppointmentVM);
            }

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


                // retrieve appointment object from DB 
                var appointmentFromDb = _db.Appointments.Where(a => a.Id == objAppointmentVM.Appointment.Id)
                    .FirstOrDefault();

                // update fields taken from the view
                appointmentFromDb.CustomerName = objAppointmentVM.Appointment.CustomerName;
                appointmentFromDb.CustomerEmail = objAppointmentVM.Appointment.CustomerEmail;
                appointmentFromDb.CustomerPhoneNumber = objAppointmentVM.Appointment.CustomerPhoneNumber;
                appointmentFromDb.AppointmentDate = objAppointmentVM.Appointment.AppointmentDate;
                appointmentFromDb.isConfirmed = objAppointmentVM.Appointment.isConfirmed;

                // check if admin to enable Sales  Person change / assign
                if (User.IsInRole(SD.SuperAdminEndUser)
                ) //TODO add Manager to be able to assign Employee to appointment 
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
        /// <param name="productPage"></param>
        /// <param name="searchName"></param>
        /// <param name="searchEmail"></param>
        /// <param name="searchPhone"></param>
        /// <param name="searchDate"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Index(int productPage = 1, string searchName = null, string searchEmail = null,
            string searchPhone = null, string searchDate = null)
        {
            // to identify what is the current user we wil use the security claims principal
            System.Security.Claims.ClaimsPrincipal curUser = this.User;

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

            if (User.IsInRole(SD.Employee)) // TODO ,  OK THIS IS WORKING NOW
            {
                // this is where we use the above created claim to retrieve user ID // 
                appointmentVM.Appointments =

                    appointmentVM.Appointments.Where(a => a.SalesPersonId == claim.Value).ToList();
            }


            // we need to do the same as we did for employee, customer should be able to view their own appointments

            // get current user from _userManager
            var custUser = await _userManager.GetUserAsync(HttpContext.User);

            // grt customer email
            var userEmail = custUser.Email;

            // if in role Customer
            if (User.IsInRole(SD.CustomerEndUser))
            {
                // this is where we use the above created claim to retrieve user ID 
                appointmentVM.Appointments =

                    // TODO, ok so the problem is that customer is not connected with appointments via regular asp created Id but we have a unique email 
                    appointmentVM.Appointments.Where(a => a.CustomerEmail == userEmail).ToList();
            }

            //if seller the same applies - seller is a user who can also have appointments , because why not? 
            if (User.IsInRole(SD.SellerEndUser))
            {
                // this is where we use the above created claim to retrieve user ID 
                appointmentVM.Appointments =

                    // TODO, ok so the problem is that customer is not connected with appointments via regular asp created Id but we have a unique email 
                    appointmentVM.Appointments.Where(a => a.CustomerEmail == userEmail).ToList();
            }



            // filter criteria for out filtering search thing 
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

            // count how many appointments there is after the search criteria

            var count = appointmentVM.Appointments.Count();

            // order and filter
            // skipping the appointments that were displayed on previous page
            // for example if we are on page two this makes sure that appointments listed on page one are not displayed on page one
            // in short - it fetches the correct records for the page we are on
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


        /// <summary>
        /// GET Details action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
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
            var productList = (IEnumerable<Products>) (from p in _db.Products
                join a in _db.ProductsSelectedForAppointments on p.Id equals a.ProductId
                where a.AppointmentId == id
                select p).Include("ProductTypes");


            // We need new AppointmentsViewModel and to populate with specific appointment data 
            AppointmentDetailsViewModel objAppointmentVM = new AppointmentDetailsViewModel()
            {

                Appointment = _db.Appointments.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefault(),
                SalesPerson = _db.ApplicationUser.ToList(), // TODO FIX THIS
                Products = productList.ToList(),


            };


            // now that we have our products we need to pass them to the view 

            return View(objAppointmentVM);
        }



        /// <summary>
        /// GET Delete action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
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



        /// <summary>
        /// POST Delete Action Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
        public async Task<IActionResult> DtleteConfirmed(int id)
        {

            // we need appointments from db
            var appointment = await _db.Appointments.FindAsync(id);
            _db.Appointments.Remove(appointment);
            await _db.SaveChangesAsync();
            TempData.Add("Delete", " You have successfully removed an appointment,... now what? ;-)");
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Appointment List - Excel File download method
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> AppList()
        {
            var appList = _db.Appointments.Include(a => a.SalesPerson);
            return View(appList);
        }


        // we can refactor this to Result to have more flexibility with returning but lest just make it simple for now
        // we shroud also specify viewModel rather than anonymous type to be "correct" but lets kep it simple for now
        // later on this can be also expanded with SQL JOIN query to include products (for example to include price of a property)
        public void AppListDownload()
        {
            //get data
            var appointmentsDw = from appointments in _db.Appointments.Include(a => a.SalesPerson)
                orderby appointments.AppointmentDate descending
                select new
                {
                    Date = appointments.AppointmentDate,
                    Name = appointments.CustomerName,
                    Email = appointments.CustomerEmail,
                    Phone = appointments.CustomerPhoneNumber,
                    SalesPerson = appointments.SalesPerson.Name,
                    Confirmed = appointments.isConfirmed,


                };




            // how many rows is there?
            int numRows = appointmentsDw.Count();

            // lets check if there is any data
            if (numRows > 0) // if there is data
            {
                // create new instance of excel package - from scratch - later it may be ideal to have a static template in DB for reuse but like I said lets keep it simple for now
                ExcelPackage excel = new ExcelPackage();

                // add excel worksheet
                var workSheet = excel.Workbook.Worksheets.Add("Appointments");

                // lets throw some data at our worksheet
                // collection; bool for Load Headers;
                workSheet.Cells[3, 1].LoadFromCollection(appointmentsDw, true);
                workSheet.Column(1).Style.Numberformat.Format = "yyyy-mm-dd HH:MM";

                //We can define block of cells Cells[startRow, startColumn, endRow, endColumn]
                workSheet.Cells[4, 1, numRows + 3, 2].Style.Font.Bold = true;

                // style heading a little - cosmetic styling
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 7])
                {
                    headings.Style.Font.Bold = true;
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.AliceBlue);
                }

                // fit columns size - autofin based on lenght of data in the cells
                workSheet.Cells.AutoFitColumns();


                // lets add title to the top
                workSheet.Cells[1, 1].Value = "Appointment Report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 6])
                {
                    Rng.Merge = true; // Merge columns start and end range
                    Rng.Style.Font.Bold = true;
                    Rng.Style.Font.Size = 18;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // time of report - time issue - time zones? server time?

                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, timeZone);
                using (ExcelRange Rng = workSheet.Cells[2, 6])
                {

                    Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " + localDate.ToShortDateString();
                    Rng.Style.Font.Bold = true;
                    Rng.Style.Font.Size = 12;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                }

                // ok, its time to download our excel file


                //one thing to bare in mind is file size and memory, on a local pc it's fine we have plenty of memory,
                //but on a server it may be an issue to load the whole thing - possible out of memory exceptions
                // what we can do to make sure we are thinking about memory is to stream the data
                // so, lets set up MemoryStream
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.Headers["content-disposition"] =
                        "attachment; filename=Appointments.xlsx"; // could ad date time to file
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.Body);
                }

            }

        }


        // TODO Add Cancel Action method POST


        
    }
}
