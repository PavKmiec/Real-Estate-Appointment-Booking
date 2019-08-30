using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace BookingWebsite.Service
{

    /// <summary>
    /// Email options - using sendGrid API - sending emails
    /// </summary>
    public class EmailOptions
    {


        /// <summary>
        /// api key
        /// </summary>
        public string SendGridKey { get; set; }
        
    }
}
