using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task AddOrder(Order order);
        Task CloseOrder(int orderid);

        Task ClosePenddingOrder();
    }
}
