using Core.IRepository;
using Core.Model;
using Repository.Data;
using Core.Infrastructure;
using Repository.UnitOfWork;
using Core.IValidation;
using System.Net;

namespace Repository.Repository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IValidation _validate;
        public OrderRepository(BaseDbContext context, IUnitOfWorkFactory unitOfWorkFactory, IValidation validate) {
            base.context = context;
            base.dbSet = context.Set<Order>();
            _unitOfWorkFactory = unitOfWorkFactory;
            _validate = validate;
        }
        public async Task AddOrder(Order order)
        {
            await base.InsertData(order);
            await SaveChangesAsync();
        }

        public async Task<Order> CloseOrder(int orderId)
        {
            var factory = _unitOfWorkFactory.create();
            var productRepo = factory.GetRepository<Product>();
            var orderRepo = factory.GetRepository<Order>();
            var orderProduct = factory.GetRepository<OrderProduct>();

            var products = productRepo.Query();
            var orderProductList = orderProduct.Query();
            Order? order = orderRepo.Query().Where(x => x.Id == orderId && (x.StatusID == (int) OrderStatus.Pedding || x.StatusID == (int) OrderStatus.Onprocess)).FirstOrDefault();

            _validate.AgainstNull(order, HttpStatusCode.NotFound, "Order ID is not valid for close order. Oder ID may not exist or already closed.");

            var priceCompareData = from op in orderProductList
                       join p in products on op.ProductID equals p.ID
                       where op.OrderID == orderId
                       select new
                       {
                           productID = op.ProductID,
                           have = p.Quantity,
                           need = op.Quantity,
                           product = p
                       };

            var list = priceCompareData.ToList();

            bool isSuccess;
            
            if(priceCompareData.Where(x => x.have < x.need).Count() > 0)
            {
                isSuccess = false;
                order.StatusID = (int) OrderStatus.Rejected;
            }
            else
            {
                isSuccess = true;
                order.StatusID = (int) OrderStatus.Approved;
                priceCompareData.ToList().ForEach(x =>
                {
                    Product obj = x.product;
                    obj.Quantity = x.have - x.need;
                    productRepo.UpdateData(obj);
                });  
            }

            await orderRepo.UpdateData(order);

            await factory.SaveChangesAsync();

            return order;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await base.GetAllAsync(null , null, x => x.OrderProduct);
        }

        public Task<Order> GetOrderByID(int id)
        {
            return base.GetByIdAsync(id).AsTask();
        }

        public async Task<IEnumerable<Order>> GetPendingOrderID()
        {
            var peddingOrder = await base.GetAllAsync(x => x.StatusID == (int)OrderStatus.Pedding);
            return peddingOrder;


        }

        public async Task<List<int>> GetPeddingRequestToClose()
        {
            var list = await this.GetPendingOrderID();
            List<int> ids = new List<int>();
            var factor = _unitOfWorkFactory.create();
            var orderRepo = factor.GetRepository<Order>();

            foreach (var order in list)
            {
                ids.Add(order.Id);
                order.StatusID = 4;
                orderRepo.UpdateData(order);
            }

            await factor.SaveChangesAsync();
            return ids;
        }
    }
}
