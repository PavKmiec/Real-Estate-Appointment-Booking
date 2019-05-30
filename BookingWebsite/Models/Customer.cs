using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models
{
    public class Customer :ApplicationUser
    {

        public bool isSeller { get; set; }
    }
}
