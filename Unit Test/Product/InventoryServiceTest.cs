using Core.IRepository;
using Core.IValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Repository.Data;
using Repository.Repository;
using Repository.UnitOfWork;
using Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Unit_Test.Product
{
    public class InventoryServiceTest
    {
        private DbContextOptions<BaseDbContext> _options;
        private BaseDbContext _dbContext;

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

            //var serviceCollection = new ServiceCollection();
            //serviceCollection.AddScoped<IEntityContext>(sp => _dbContext);
            //serviceCollection.AddTransient(typeof(IRepository<>), typeof(GenericRepository<>));
            //serviceCollection.AddScoped<IInvertoryRepository, IInvertoryRepository>();

            //var serviceProvider = serviceCollection.BuildServiceProvider();
            
        }

        [Test]
        public void AddProduct_AddTest()
        {
            var repo = new InventoryRepository(_dbContext);
            var service = new InventoryService(repo);

            service.AddProduct(new Core.Model.Product()
            {
                Name = "Ipod",
                Quantity = 5
            });

            var products = _dbContext.Product;
            var product = products.Where(x => x.Name == "Ipod" && x.Quantity == 5).ToList();
            Assert.IsNotEmpty(product);
        }

        [Test]
        public void DeleteProduct_deleteExistingProduct()
        {
            var product = _dbContext.Product.Add(new Core.Model.Product()
            {
                Name = "Delete",
                Quantity = 5
            });

            _dbContext.SaveChanges();

            var repo = new InventoryRepository(_dbContext);
            var service = new InventoryService(repo);
            service.DeleteProduct(product.Entity.ID);

            var deletedData = _dbContext.Product.Find(product.Entity.ID);
            Assert.IsNull(deletedData);

        }
    }
}
