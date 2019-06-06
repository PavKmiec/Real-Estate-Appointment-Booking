using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models.ViewModel
{
    public class AppointmentDetailsViewModel
    {
        /// <summary>
        /// Appointment - Appointments type
        /// </summary>
        public Appointments Appointment { get; set; }


        /// <summary>
        /// We need this to display staff in drop down list so staff can be assigned to appointment 
        /// </summary>
        public List<ApplicationUser> SalesPerson { get; set; }

        /// <summary>
        /// We also need list of products fro this view model to display beside appointment details
        /// </summary>
        public List<Products> Products { get; set; }




    }
}
