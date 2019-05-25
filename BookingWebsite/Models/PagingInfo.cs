using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models
{
    /// <summary>
    /// This class is for custom made asp-helper for pagination that we can use in our views that display lists
    /// </summary>
    public class PagingInfo
    {
        public int TotalItems { get; set; }

        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }

        // total items by items per page
        public int totalPage => (int) Math.Ceiling((decimal) TotalItems / ItemsPerPage);

        // url parameter - will be used to build url
        public string urlParam { get; set; }

    }
}
