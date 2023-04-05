using AutoMapper;
using BackendAPI.Controllers;
using BackendAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repository;
using Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test.Product
{
    public class InvetoryAPITest
    {
        private DbContextOptions<BaseDbContext> _options;
        private BaseDbContext _dbContext;
        private InventoryController _controller;
        private IMapper _mapper;
        [SetUp]
        public void Setup()
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

            var repo = new InventoryRepository(_dbContext);
            var service = new InventoryService(repo);
            _controller = new InventoryController(service, mapper);
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
        public async Task AddProductAPITest()
        {
            BackendAPI.Model.Product product = new BackendAPI.Model.Product() { 
                Name = "Test",
                Quantity = 10,
            };

            BackendAPI.Model.Product product1 = new BackendAPI.Model.Product()
            {
                Name = "Test1",
                Quantity = 10,
            };
            var countBeforeSave = _dbContext.Product.Where(x => x.Name == "Test" && x.Quantity == 10).Count();   
            int isSaved = await _controller.Post(product);
            var countAfter = _dbContext.Product.Where(x => x.Name == "Test" && x.Quantity == 10).Count();

            Assert.That(isSaved, Is.EqualTo(1));
            Assert.That(countBeforeSave, Is.EqualTo(countAfter - 1));
        }

        [Test]
        public async Task DeleteProductApiTest() 
        {
            int id = _dbContext.Product.FirstOrDefault().ID;
            await _controller.Delete(id);

            var product = _dbContext.Product.Find(id);
            Assert.IsNull(product);
        }
    }
}
