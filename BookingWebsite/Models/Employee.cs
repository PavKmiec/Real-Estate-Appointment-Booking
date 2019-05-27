using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace BookingWebsite.Models
{
    public class Employee
    {

        public string Grade { get; set; }
        public bool isManager { get; set; }

        public int BranchId { get; set; }


    }
}
