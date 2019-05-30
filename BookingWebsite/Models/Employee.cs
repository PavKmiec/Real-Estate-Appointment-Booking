using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace BookingWebsite.Models
{
    public class Employee : ApplicationUser
    {

        public string Grade { get; set; }
        public bool isManager { get; set; }

        public int BranchId { get; set; }


        public virtual Branch Branches { get; set; }

        //public Employee()
        //{
        //    Branch branch = new Branch
        //    {
        //        Name = "Main",
        //        Location = "Glasgow",
        //    };




        }


    }

