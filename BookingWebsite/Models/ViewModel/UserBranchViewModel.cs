using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models.ViewModel
{
    public class UserBranchViewModel
    {

        public List<ApplicationUser> UserList { get; set; }

        public List<Branch> BranchList { get; set; }

        // or could do: public Branch Branch, and add public List<string> BranchList - in case we would only need Branch Name (one property from model)
    }
}
