using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models
{
    /// <summary>
    /// View Model for products selected for appointment, Appointment + Products 
    /// </summary>
    public class ProductsSelectedForAppointment
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }

        // Foreign ky reference, for Appointments table
        [ForeignKey("AppointmentId")]
        public virtual Appointments Appointments { get; set; }


        public int ProductId { get; set; }

        // Foreign ky reference, for Products table
        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }
    }
}
