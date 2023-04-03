using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Model
{
    public class OrderProduct
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }
}
