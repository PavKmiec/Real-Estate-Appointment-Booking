using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models
{
    /// <summary>
    /// Product Tags Model
    /// </summary>
    public class Tags
    {
        /// <summary>
        /// Id used by EF
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// Tag Name
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
