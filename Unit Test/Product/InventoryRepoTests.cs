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
                List<Core.Model.Product>? list = invertoryRepo.GetAllAsync().Result.ToList();
                Assert.AreEqual(2, list.Count);

            }

            //var mockSet = new Mock<DbSet<Product>>();
            //var mockContext = new Mock<BaseDbContext>();
            //mockContext.Setup(m => m.Product).Returns(mockSet.Object);

            //var invertoryRepo = new InventoryRepository(mockContext.Object);
            //invertoryRepo.AddProduct( new Product() { 
            //    Name = "Apple",
            //    Quantity = 100
            //});

            //mockSet.Verify(m => m.Add(It.IsAny<Product>()), Times.Once());
            //mockContext.Verify(m => m.SaveChanges(), Times.Once());
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