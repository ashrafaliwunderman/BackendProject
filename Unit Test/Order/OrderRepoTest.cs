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
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test.Order
{
    public class OrderRepoTest
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
                Quantity = 1
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

        //[Test]
        //public void GetAllOrders_TotalCountTest() 
        //{
        //    var product = _dbContext.Product.Where(x => x.Name == "Airpod pro 2").FirstOrDefault();
        //    var id = product.ID;


        //    _dbContext.Order.Add(new Core.Model.Order()
        //    {
        //        CustomerEmail = "test@gmail.com",
        //        OrderProduct = new List<OrderProduct>() {
        //            new OrderProduct(){
        //                ProductID = id,
        //                Quantity = 10
        //            }
        //        }
        //    });

        //    _dbContext.SaveChanges();

        //    List<Core.Model.Order>? list = _orderRepo.GetAllOrders().Result.ToList();
        //    Assert.That(list.Count, Is.EqualTo(2));
        //    Assert.That(list[0].OrderProduct.Count, Is.EqualTo(1));
        //}

        [Test]
        public async Task AddOrder_TotalCountTest()
        {
            var product = _dbContext.Product.Where(x => x.Name == "Airpod pro 2").FirstOrDefault();
            var id = product.ID;

            var order = new Core.Model.Order()
            {
                CustomerEmail = "test@gmail.com",
                OrderProduct = new List<OrderProduct>() {
                    new OrderProduct(){
                        ProductID = id,
                        Quantity = 10
                    }
                },
                StatusID = (int) OrderStatus.Pedding
            };

            await _orderRepo.AddOrder(order);
            var count = _dbContext.Order.Count();
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetPendingOrderID_TestReturnID()
        {
            var actualIDList = _dbContext.Order.Where(x => x.StatusID == (int) OrderStatus.Pedding).Select(x => x.Id).ToList();
            var repoReturnIDList = _orderRepo.GetPendingOrder().Result.Select(x => x.Id).ToList();
            Assert.That(repoReturnIDList.Count, Is.EqualTo(actualIDList.Count));

            var commaSeparatedString = string.Join(',', repoReturnIDList);
            Assert.That(String.Join( ',', actualIDList), Is.EqualTo(commaSeparatedString));
        }

        [Test]
        public void GetPeddingOrderIDs_TestIDList()
        {
            var actualIDList = _dbContext.Order.Where(x => x.StatusID == (int)OrderStatus.Pedding).Select(x => x.Id).ToList();
            var repoReturnIDList = _orderRepo.GetPeddingOrderIDs().Result.ToList();
            Assert.That(repoReturnIDList.Count, Is.EqualTo(actualIDList.Count));

            var commaSeparatedString = string.Join(',', repoReturnIDList);
            Assert.That(String.Join(',', actualIDList), Is.EqualTo(commaSeparatedString));
        }

        [Test]
        public void GetPeddingOrderIDs_TestUpdateStatus()
        {
            var repoReturnIDList = _orderRepo.GetPeddingOrderIDs().Result.ToList();
            var actualIDList = _dbContext.Order.Where(x => x.StatusID == (int)OrderStatus.Onprocess).Select(x => x.Id).ToList();

            foreach (var id in repoReturnIDList)
            {
                Assert.IsNotEmpty(actualIDList.Where(x => x == id));
            }
        }

        [Test]

        public void GetOrderByID_TestSameObject()
        {
            var actualIDList = _dbContext.Order.Include(x => x.OrderProduct);

            foreach (var data in actualIDList)
            {
                Core.Model.Order order = _orderRepo.GetOrderByID(data.Id).Result;

                Assert.That(order.CustomerEmail, Is.EqualTo(data.CustomerEmail));
                Assert.That(order.StatusID, Is.EqualTo(data.StatusID));
                Assert.That(order.OrderProduct[0].Quantity, Is.EqualTo(data.OrderProduct[0].Quantity));
            }

        }

        [Test]
        public void GetAllOrders_TestSameObject()
        {
            var actualIDList = _dbContext.Order.Include(x => x.OrderProduct);
            var allProduct = _orderRepo.GetAllOrders().Result;
            foreach (var data in actualIDList)
            {
                Core.Model.Order order = allProduct.Where(x => x.Id == data.Id).First();

                Assert.That(order.CustomerEmail, Is.EqualTo(data.CustomerEmail));
                Assert.That(order.StatusID, Is.EqualTo(data.StatusID));
                Assert.That(order.OrderProduct[0].Quantity, Is.EqualTo(data.OrderProduct[0].Quantity));
            }

        }
    }
}
