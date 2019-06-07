using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models.ViewModel
{
    /// <summary>
    /// Products View Model - to combine Products with Types and Tags
    /// </summary>
    public class ProductsViewModel
    {
        /// <summary>
        /// Products 
        /// </summary>
        public Products Products { get; set; }

        /// <summary>
        /// ProductsTypes
        /// </summary>
        [Display(Name = "Property Type")]
        public IEnumerable<ProductTypes> ProductTypes { get; set; }


        /// <summary>
        /// Tags
        /// </summary>
        [Display(Name = "Tag")]
        public IEnumerable<Tags> Tags { get; set; }



    }
}
