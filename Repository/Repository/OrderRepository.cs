using Core.IRepository;
using Core.Model;
using Repository.Data;
using Core.Infrastructure;
using Repository.UnitOfWork;
using Core.IValidation;
using System.Net;
using Repository.Model;
using System.Runtime.InteropServices;

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
                       select new ProductOrderQuantity()
                       {
                           ProductID = op.ProductID,
                           InventoryHave = p.Quantity,
                           OrderNeed = op.Quantity,
                           product = p
                       };

            var list = priceCompareData.ToList();

            
            if(IsOrderQuantityAvailable(priceCompareData, ref order))
            {
                UpdateProductQuatity(priceCompareData, ref order, productRepo);
            }

            await orderRepo.UpdateData(order);

            await factory.SaveChangesAsync();

            return order;
        }

        private Order UpdateProductQuatity(IQueryable<ProductOrderQuantity> productOrderQuantity, ref Order? order, IRepository<Product> productRepo) {

            order.StatusID = (int)OrderStatus.Approved;
            productOrderQuantity.ToList().ForEach(x =>
            {
                Product obj = x.product;
                obj.Quantity = x.InventoryHave - x.OrderNeed;
                productRepo.UpdateData(obj);
            });
            return order;
        }

        private bool IsOrderQuantityAvailable(IQueryable<ProductOrderQuantity> obj, ref Order? order)
        {
            if (obj.Where(x => x.InventoryHave < x.OrderNeed).Count() > 0)
            {
                order.StatusID = (int)OrderStatus.Rejected;
                return false;
            }

            return true;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await base.GetAllAsync(null , null, x => x.OrderProduct);
        }

        public Task<Order> GetOrderByID(int id)
        {
            return base.GetByIdAsync(id).AsTask();
        }

        public async Task<IEnumerable<Order>> GetPendingOrder()
        {
            var peddingOrder = await base.GetAllAsync(x => x.StatusID == (int)OrderStatus.Pedding);
            return peddingOrder;
        }

        public async Task<List<int>> GetPeddingOrderIDs()
        {
            var list = await this.GetPendingOrder();
            List<int> ids = new List<int>();
            var factor = _unitOfWorkFactory.create();
            var orderRepo = factor.GetRepository<Order>();

            foreach (var order in list)
            {
                ids.Add(order.Id);
                order.StatusID = (int) OrderStatus.Onprocess;
                orderRepo.UpdateData(order);
            }

            await factor.SaveChangesAsync();
            return ids;
        }
    }
}
