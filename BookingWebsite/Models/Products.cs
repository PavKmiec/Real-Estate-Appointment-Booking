using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookingWebsite.Models
{
    /// <summary>
    /// Products Model
    /// </summary>
    public class Products
    {

        /// <summary>
        /// Id used by EF
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Product Name
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Price
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Availability for sale
        /// </summary>
        public bool Available { get; set; }


        /// <summary>
        /// image location
        /// </summary>
        public string Image { get; set; }


        /// <summary>
        /// Product description
        /// </summary>
        public string Description { get; set; }


        // Furnish detail - (Furnished, unfurnished, part furnished)
        public string FurnishDetail { get; set; }



        [Display(Name = "Product Type")]
        public int ProductTypeId { get; set; }


        /// <summary>
        /// Linking ProductTypes to ProductTypeId as foreign key
        /// </summary>
        [ForeignKey("ProductTypeId")]
        public virtual ProductTypes ProductTypes { get; set; }




        [Display(Name = "Tag")]
        public int TagsId { get; set; }


        /// <summary>
        /// Linking Tags to TagsId as foreign key
        /// </summary>
        [ForeignKey("TagsId")]
        public virtual Tags Tags { get; set; }



        /// <summary>
        /// Linking product with user - for sellers to have products
        /// </summary>
        public int? UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
