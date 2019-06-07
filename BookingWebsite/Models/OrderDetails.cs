using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace BookingWebsite.Models
{
    //TODO
    /// <summary>
    /// This is yet to be implemented - this is for the seller to be able to pay 1% fee when  seller user adds a product to the system
    /// </summary>
    public class OrderDetails
    {
        /// <summary>
        /// Id used by EF
        /// </summary>
        [Key]
        public int Id { get; set; }


        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderHeader OrderHeader { get; set; }


        [Required]
        public int ProductsId { get; set; }

        [ForeignKey("ProductsId")]
        public virtual Products Products { get; set; }


        public string Name { get; set; }

        public string Description { get; set; }


        [Required]
        public double Price { get; set; }






    }
}
