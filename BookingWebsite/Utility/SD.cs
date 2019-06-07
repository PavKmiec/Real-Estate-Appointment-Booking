using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Utility
{

    /// <summary>
    /// SD - Static Details Class to hold static values that are reused in the system
    /// </summary>
    public class SD
    {
        /// <summary>
        /// default image - if none uploaded
        /// </summary>
        public const string DefaultProductImage = "default_image.png";

        /// <summary>
        /// image folder
        /// </summary>
        public const string ImageFolder = @"images\ProductImage";


        // storing roles so we don't have to use magic strings in the project
        public const string SuperAdminEndUser = "Admin";
        public const string AdminEndUser = "Manager";
        public const string Employee = "Employee";
        public const string CustomerEndUser = "Customer";
        public const string SellerEndUser = "Seller";


    }
}
