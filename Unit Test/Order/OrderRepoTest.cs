using Core.Model;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test.Order
{
    public class OrderRepoTest
    {
        [Test]
        public void GetAllOrders_TotalCountTest() 
        {
            var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseInMemoryDatabase(databaseName: "orderDBTest")
            .Options;

            using (var context = new BaseDbContext(options))
            {
                context.Product.Add(new Core.Model.Product()
                {
                    Name = "Airpod pro 2",
                    Quantity = 1
                });


                context.Order.Add(new Core.Model.Order()
                {
                    CustomerEmail = "test@gmail.com",
                    OrderProduct = new List<OrderProduct>() { 
                        new OrderProduct(){
                            ProductID = 1,
                            Quantity = 10
                        }
                    }
                });

               
                context.SaveChanges();
            }

            //using (var context = new BaseDbContext(options))
            //{
            //    var invertoryRepo = new OrderRepository(context);
            //    List<Core.Model.Product>? list = invertoryRepo.GetAllAsync().Result.ToList();
            //    Assert.AreEqual(2, list.Count);

            //}
        }
    }
}
