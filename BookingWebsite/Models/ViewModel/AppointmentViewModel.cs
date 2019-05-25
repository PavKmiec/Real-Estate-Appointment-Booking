using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models.ViewModel
{

    /// <summary>
    /// Appointment VM 
    /// </summary>
    public class AppointmentViewModel
    {

        // We need a list of appointments 
        /// <summary>
        /// List of appointments - IEnum of type Appointments
        /// </summary>
        public IEnumerable<Appointments> Appointments { get; set; }


        // paging
        public PagingInfo PagingInfo { get; set; }



    }
}
