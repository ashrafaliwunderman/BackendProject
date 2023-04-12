using Core.Infrastructure;
using Core.IRepository;
using Core.IValidation;
using Core.Model;
using Core.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository.Data;
using Repository.Repository;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test.Order
{
    public class OrderRepsoCloseOrderTest
    {
        private DbContextOptions<BaseDbContext> _options;
        private BaseDbContext _dbContext;
        private OrderRepository _orderRepo;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseInMemoryDatabase(databaseName: "orderDBTest")
            .Options;

            _dbContext = new BaseDbContext(_options);


            _dbContext.Product.Add(new Core.Model.Product()
            {
                Name = "Airpod pro 2",
                Quantity = 10
            });

            _dbContext.SaveChanges();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IEntityContext>(sp => _dbContext);
            serviceCollection.AddTransient(typeof(IRepository<>), typeof(GenericRepository<>));

            var serviceProvider = serviceCollection.BuildServiceProvider();
            UnitOrWorkFactory factory = new UnitOrWorkFactory(serviceProvider);
            IValidation validation = new Validation();
            _orderRepo = new OrderRepository(_dbContext, factory, validation);
        }

        [Test]
        public void CloseOrder_TestStatusAfterClose()
        {
            var product = _dbContext.Product.Where(x => x.Name == "Airpod pro 2").FirstOrDefault();
            var id = product.ID;

            product.Quantity = 11;
            _dbContext.Update(product);
            _dbContext.SaveChanges();

            var orderModel = new Core.Model.Order()
            {
                CustomerEmail = "valid@gmail.com",
                OrderProduct = new List<OrderProduct>() {
                    new OrderProduct(){
                        ProductID = id,
                        Quantity = 10
                    }
                },
                StatusID = (int)OrderStatus.Pedding
            };

            _dbContext.Order.Add(orderModel);
            _dbContext.SaveChanges();

            var pendingOrder = _dbContext.Order.Where(x => x.StatusID == (int)OrderStatus.Pedding && x.CustomerEmail == "valid@gmail.com");
            var orderid = pendingOrder.FirstOrDefault().Id;
            var order = _orderRepo.CloseOrder(orderid);

            var statusid = _dbContext.Order.Find(orderid).StatusID;

            Assert.True(statusid == (int) OrderStatus.Approved);
        }

        [Test]
        public void CloseOrder_RejectDuetoQuantity()
        {
            var product = _dbContext.Product.Where(x => x.Name == "Airpod pro 2").FirstOrDefault();
            var id = product.ID;

            var orderModel = new Core.Model.Order()
            {
                CustomerEmail = "invalid@gmail.com",
                OrderProduct = new List<OrderProduct>() {
                    new OrderProduct(){
                        ProductID = id,
                        Quantity = 100
                    }
                },
                StatusID = (int)OrderStatus.Pedding
            };

            _dbContext.Order.Add(orderModel);
            _dbContext.SaveChanges();

            var pendingOrder = _dbContext.Order.Where(x => x.StatusID == (int)OrderStatus.Pedding && x.CustomerEmail == "invalid@gmail.com");
            var orderid = pendingOrder.FirstOrDefault().Id;
            var order = _orderRepo.CloseOrder(orderid);

            var statusid = _dbContext.Order.Find(orderid).StatusID;

            Assert.True(statusid == (int)OrderStatus.Rejected);
        }
    }
}
