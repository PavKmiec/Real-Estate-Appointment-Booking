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



        
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }



        [NotMapped]
        public bool IsSuperAdmin { get; set; }
    }
}
