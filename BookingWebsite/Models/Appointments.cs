using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models
{
    /// <summary>
    /// Appointmants model class
    /// </summary>
    public class Appointments
    {
        /// <summary>
        /// Appointment Id, used by EF
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// we need to link sales person for appointment
        /// </summary>
        [Display(Name = "Sales Person")]
        public string SalesPersonId { get; set; }

        /// <summary>
        /// Foreign Key reference pointing to SalesPersonId
        /// </summary>
        [ForeignKey("SalesPersonId")]
        public virtual ApplicationUser SalesPerson { get; set; }

        /// <summary>
        /// Appointment date
        /// </summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        /// Appointment Time - we do not need tis in database as we can use "Date" property to store time too in our DB
        /// </summary>
        [NotMapped]
        public DateTime AppointmentTime { get; set; }


        /// <summary>
        /// Customer name
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Customer phone
        /// </summary>
        public string CustomerPhoneNumber { get; set; }

        /// <summary>
        /// customer email
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// is appointment confirmed - bool
        /// </summary>
        public bool isConfirmed { get; set; }



    }
}
