using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Model
{
    public class ProductOrderQuantity
    {
        public int ProductID { get; set; }
        public int InventoryHave { get; set; }
        public int OrderNeed { get; set; }
        public Product product { get; set; }
    }
}
