using Core.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Model
{
    public class Order
    {
        public List<Model.OrderProduct>? OrderProduct { get; set; }
        [EmailAddress]
        public string CustomerEmail { get; set; }

        public int StatusID { get; set; }
    }
}
