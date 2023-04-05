using Core.IRepository;
using Core.Model;
using Core.PublishService;
using Service.Interface;

namespace Service.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IEmailQueueService _emailQueueService;
        private readonly IPushOrderService _closePenddingOrderService;
        public OrderService(IOrderRepository repo, IEmailQueueService emailQueueService, IPushOrderService closerPeddingOrderService)
        {
            _orderRepo = repo;
            _emailQueueService = emailQueueService;
            _closePenddingOrderService = closerPeddingOrderService;
        }
        public async Task AddOrder(Order order)
        {
            await _orderRepo.AddOrder(order);
        }

        public async Task CloseOrder(int orderid)
        {

            var order = await _orderRepo.CloseOrder(orderid);
            var email = order.GetMail();
            await _emailQueueService.PushQueue(email);
        }

        public async Task ClosePenddingOrder()
        {
            List<int> list = await _orderRepo.GetPeddingOrderIDs();
            List<Task> tasks = new List<Task>();
            foreach (var item in list)
            {
                var publisher = _closePenddingOrderService.CloseOrderRequestPublisher(item);
                tasks.Add(publisher);
            }
            await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _orderRepo.GetAllOrders();
        }
    }
}
