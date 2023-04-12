using Microsoft.EntityFrameworkCore;
using Moq;
using Repository.Data;
using Repository.Repository;

namespace Unit_Test.Product
{
    public class InventoryRepoTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ProductAddTest()
        {
            var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseInMemoryDatabase(databaseName: "BackendMockAdd")
            .Options;

            using (var context = new BaseDbContext(options))
            {

                context.Product.Add(new Core.Model.Product()
                {
                    Name = "Airpod pro 2",
                    Quantity = 1
                });

                context.Product.Add(new Core.Model.Product()
                {
                    Name = "Linkbuds s",
                    Quantity = 2
                });
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new BaseDbContext(options))
            {
                var invertoryRepo = new InventoryRepository(context);
                List<Core.Model.Product>? list = invertoryRepo.GetAllAsync().Result.ToList();
                Assert.AreEqual(2, list.Count);

            }
        }

        [Test]
        public void ProductDeleteTest()
        {
            var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseInMemoryDatabase(databaseName: "BackendMock")
            .Options;

            using (var context = new BaseDbContext(options))
            {

                context.Product.Add(new Core.Model.Product()
                {
                    Name = "Airpod pro 2",
                    Quantity = 1
                });

                context.Product.Add(new Core.Model.Product()
                {
                    Name = "Linkbuds s",
                    Quantity = 2
                });
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new BaseDbContext(options))
            {
                var invertoryRepo = new InventoryRepository(context);
                invertoryRepo.DeleteProduct(1);
                List<Core.Model.Product>? list = invertoryRepo.GetAllAsync().Result.ToList();
                list = list.Where(x => x.ID == 1).ToList();
                Assert.IsEmpty(list);
            }
        }

        [Test]
        public void ProductUpdateTest()
        {
            var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseInMemoryDatabase(databaseName: "BackendMock")
            .Options;

            using (var context = new BaseDbContext(options))
            {

                context.Product.Add(new Core.Model.Product()
                {
                    Name = "Airpod pro 2",
                    Quantity = 1
                });

                context.Product.Add(new Core.Model.Product()
                {
                    Name = "Linkbuds s",
                    Quantity = 2
                });
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new BaseDbContext(options))
            {
                var invertoryRepo = new InventoryRepository(context);
                Core.Model.Product product = invertoryRepo.GetAllAsync().Result.FirstOrDefault();
                product.Name = "updated";
                var saveID = product.ID;
                invertoryRepo.UpdateProduct(product);
                var updatedProduct = invertoryRepo.GetByIdAsync(saveID).Result;
                Assert.AreEqual("updated", updatedProduct.Name);
            }
        }
    }
}