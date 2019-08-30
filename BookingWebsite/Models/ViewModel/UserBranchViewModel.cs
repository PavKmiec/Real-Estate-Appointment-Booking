using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models.ViewModel
{
    //TODO
    /// <summary>
    /// View Model to use in registration - to make selecting Branch possible in case of adding employees
    /// This is yet to be implemented - low priority
    /// </summary>
    public class UserBranchViewModel
    {

        public List<ApplicationUser> UserList { get; set; }

        public List<Branch> BranchList { get; set; }

    }
}
