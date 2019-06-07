using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models
{
    /// <summary>
    /// Branch model - employee can belong to a branch of a company
    /// </summary>
    public class Branch
    {
        /// <summary>
        /// ID used by EF
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Branch name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Branch location
        /// </summary>
        [Required]
        public string Location { get; set; }


        /// <summary>
        /// User
        /// </summary>
        public ICollection<ApplicationUser> ApplicationUser { get; set; }



    }

}


    

