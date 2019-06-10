using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models.ViewModel
{
    /// <summary>
    /// Shopping Cart View Model - 
    /// </summary>
    public class ShoppingCartViewModel
    {
        /// <summary>
        /// list of products
        /// </summary>
        public List<Products> Products { get; set; }


        /// <summary>
        /// appointments
        /// </summary>
        public Appointments Appointments { get; set; }




        public ApplicationUser CustomerUser { get; set; }
    }
}
