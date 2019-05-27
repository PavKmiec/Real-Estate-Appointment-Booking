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

        public const string DefaultProductImage = "default_image.png";
        public const string ImageFolder = @"images\ProductImage";


        // roles
        public const string AdminEndUser = "Admin";
        public const string SuperAdminEndUser = "Super Admin";
        public const string CustomerEndUser = "Customer";

    }
}
