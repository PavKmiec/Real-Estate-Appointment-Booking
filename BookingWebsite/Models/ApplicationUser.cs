using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BookingWebsite.Models
{
    public class ApplicationUser : IdentityUser
    {




        [Required]
        [StringLength(60)]
        public string Name { get; set; }
        [Required]
        [Display(Name = " Address")]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

        /// <summary>
        /// Employee properties - because we use one table for all users those can be null in case user is not employee
        /// </summary>
        [Display(Name = "Pay Grade")]
        public string Grade { get; set; }



        public int? BranchId { get; set; }
        public Branch Branch { get; set; }



        public ICollection<Products> Products { get; set; }


        [Display(Name = "Appointment")]
        public int? AppointmentId { get; set; }


        /// <summary>
        /// Linking ProductTypes to ProductTypeId as foreign key
        /// </summary>
        [ForeignKey("AppointmentId")]
        public virtual Appointments Appointments { get; set; }






        //[NotMapped]
        //public bool IsSuperAdmin { get; set; }
    }
}
