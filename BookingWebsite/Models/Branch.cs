using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models
{
    public class Branch
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }


        public ICollection<ApplicationUser> ApplicationUser { get; set; }



    }

}


    

