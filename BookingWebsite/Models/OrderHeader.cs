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
    /// order header contains important details about order like when was it placed, order total, paymrnt status, user who placed order
    /// </summary>
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }


        [Required] public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Order Total")]
        public double OrderTotal { get; set; }


        [Required]
        [Display]
        public DateTime AppointmentDate { get; set; }

        public string Status { get; set; }
        public string PaymentStatus { get; set; }

        public string ContactPhone { get; set; }

        public string TransactionId { get; set; }

    }
}
