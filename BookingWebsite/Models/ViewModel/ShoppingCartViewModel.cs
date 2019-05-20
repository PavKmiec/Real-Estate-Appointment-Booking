using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models.ViewModel
{
    public class ShoppingCartViewModel
    {
        // for this Shopping cart ViewModel we need list of products as well as we need Appointments model
        // list of products
        public List<Products> Products { get; set; }

        // appointments
        public Appointments Appointments { get; set; }
    }
}
