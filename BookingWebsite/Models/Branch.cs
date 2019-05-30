using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models
{
    public class Branch
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }



        public virtual ICollection<Employee> Employees { get; set; }

        public Branch()
        {

            Employees = new List<Employee>(); 
        }
        
    }


    
}
