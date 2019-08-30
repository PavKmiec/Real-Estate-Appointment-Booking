using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Models
{

    /// <summary>
    /// Model for product typer (House / Flat / Tent/ Boat)
    /// </summary>
    public class ProductTypes
    {
        /// <summary>
        /// Id used by EF, PK
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Product type name
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
