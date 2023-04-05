using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IRepository
{
    public interface IOrderRepository 
    {
        Task<IEnumerable<Order>> GetAllOrders();
        Task AddOrder(Order order);
        Task<Order> CloseOrder(int orderId);

        Task<Order> GetOrderByID(int id);

        Task<IEnumerable<Order>> GetPendingOrder();
        Task<List<int>> GetPeddingOrderIDs();
        
    }
}
