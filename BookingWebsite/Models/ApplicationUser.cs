using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BookingWebsite.Models
{
    /// <summary>
    /// Application user model - inheriting from IdentityUser
    /// Additional properties for users are here 
    /// </summary>
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


        /// <summary>
        /// user in Employee role can belong to a branch
        /// </summary>
        public int? BranchId { get; set; }
        public Branch Branch { get; set; }


        /// <summary>
        /// Products
        /// </summary>
        public ICollection<Products> Products { get; set; }


        // link appointments 



        
    }
}
