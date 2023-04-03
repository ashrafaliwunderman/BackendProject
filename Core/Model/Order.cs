using Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class Order
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public List<OrderProduct> OrderProduct { get; set; }
        [EmailAddress]
        public string CustomerEmail { get; set; } = default!;
        [Required]

        [ForeignKey("Status")]
        public int StatusID { get; set;}
        public DateTime CreatedDate { get; set; }

        private Email.Email GetRejectedEmail()
        {
            Email.Email email = new Email.Email();
            email.To = this.CustomerEmail;
            email.Subject = "You order has been cancled.";
            email.Body = "Your ordrer has been cancled.";
            email.Body += "Your order ID: " + this.Id;
            return email;
        }

        private Email.Email GetOrderSuccessEmail()
        {
            Email.Email email = new Email.Email();
            email.To = this.CustomerEmail;
            email.Subject = "Order Processed.";
            email.Body = "Your order is processed successfully.";
            email.Body += "Your order ID: " + this.Id;
            return email;
        }

        public Email.Email GetMail()
        {
            if (this.StatusID == (int) OrderStatus.Approved)
            {
                return GetOrderSuccessEmail();
            }
            else
            {
                return GetRejectedEmail();
            }
        }
    }
}
