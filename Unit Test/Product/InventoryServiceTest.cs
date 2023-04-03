using Core.IRepository;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repository;
using Service.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test.Product
{
    public class InventoryServiceTest
    {
        [Test]
        public void ProductServiceAddTest()
        {
            var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseInMemoryDatabase(databaseName: "BackendMock")
            .Options;

            var context = new BaseDbContext(options);
            var repo = new InventoryRepository(context);
            var service = new InventoryService(repo);

            service.AddProduct(new Core.Model.Product()
            {
                Name = "Ipod",
                Quantity = 5
            });

            var products = repo.GetAllAsync().Result.ToList();
            var product = products.Where(x => x.Name == "Ipod" && x.Quantity == 5).ToList();
            Assert.IsNotEmpty(product);
        }
    }
}
