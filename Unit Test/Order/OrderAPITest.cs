using AutoMapper;
using BackendAPI.Controllers;
using BackendAPI.Infrastructure;
using BackendAPI.Model;
using Core.Infrastructure;
using Core.IRepository;
using Core.IValidation;
using Core.Model;
using Core.Validation;
using MessageQueuePubService.ClosePeddingOrderService;
using MessageQueuePubService.EmailRequestService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Repository.Data;
using Repository.Repository;
using Repository.UnitOfWork;
using Service.Interface;
using Service.Service;

namespace Unit_Test.Order
{
    public class OrderAPITest
    {

        private DbContextOptions<BaseDbContext> _options;
        private BaseDbContext _dbContext;
        private OrderController _controller;
        private IMapper _mapper;

        [SetUp]

        public void SetUp()
        {
            _options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseInMemoryDatabase(databaseName: "BackendMock")
            .Options;

            _dbContext = new BaseDbContext(_options);
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IEntityContext>(sp => _dbContext);
            serviceCollection.AddTransient(typeof(IRepository<>), typeof(GenericRepository<>));

            var serviceProvider = serviceCollection.BuildServiceProvider();
            UnitOrWorkFactory factory = new UnitOrWorkFactory(serviceProvider);
            IValidation validation = new Validation();


            var repo = new OrderRepository(_dbContext, factory, validation);

            var emailServiceQueue = new EmailQueueService();
            var pushOrderService = new PushOrderService();
            var service = new OrderService(repo, emailServiceQueue, pushOrderService);
            _controller = new OrderController(service, mapper);

            SeedData();
        }

        private void SeedData()
        {
            _dbContext.Product.Add(new Core.Model.Product()
            {
                Name = "Test",
                Quantity = 10,
            });
            _dbContext.SaveChanges();
        }

        [Test]

        public void AddOrder_InsertSameOrderIDmultiTime()
        {
            var productID = _dbContext.Product.Where( x => x.Name == "Test" && x.Quantity == 10 ).FirstOrDefault()?.ID;

            var order = new BackendAPI.Model.Order()
            {
                CustomerEmail = "tet@gmail.com",
                OrderProduct = new List<BackendAPI.Model.OrderProduct>(){
                    new BackendAPI.Model.OrderProduct()
                    {
                        ProductID =  productID?? 1,
                        Quantity = 2
                    },
                    new BackendAPI.Model.OrderProduct() {
                        ProductID = productID?? 1,
                        Quantity = 2
                    }
                },
                StatusID = (int) OrderStatus.Pedding
            };

            _controller.Post(order);

            var productOrder = _dbContext.OrderProduct.Where(x => x.ProductID == productID).ToList();
            Assert.That(productOrder.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAllOrderPost_test()
        {
            var orderService = new Mock<IOrderService>();

            var list = new List<Core.Model.Order>()
            {
                new Core.Model.Order(){
                    CustomerEmail = "tet@gmail.com",
                    OrderProduct = new List<Core.Model.OrderProduct>(){
                        new Core.Model.OrderProduct()
                        {
                            ProductID = 1,
                            Quantity = 2
                        },
                        new Core.Model.OrderProduct() {
                            ProductID = 2,
                            Quantity = 5
                        }
                    },
                    StatusID = (int)OrderStatus.Pedding

                }
            };
            orderService.Setup(x => x.GetAllOrders())
                .Returns(Task.FromResult<IEnumerable<Core.Model.Order>>(list.AsEnumerable()));


            var orderController = new OrderController(orderService.Object, _mapper);

            var data = await orderController.Get();

            Assert.That(data.Count(), Is.EqualTo(1));
            Assert.That(data.ToList()[0].OrderProduct.Count(), Is.EqualTo(2));
        }

    }
}
