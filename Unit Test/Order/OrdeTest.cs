using Core.Infrastructure;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test.Order
{
    public class OrdeTest
    {
        [Test]
        public void GetMail_getRejectedMail()
        {
            Core.Model.Order order = new Core.Model.Order();
            order.StatusID = (int) OrderStatus.Rejected;

            var Email = order.GetMail();
            Assert.AreEqual("You order has been cancled.", Email.Subject);
        }

        [Test]
        public void GetMail_getSuccessMail()
        {
            Core.Model.Order order = new Core.Model.Order();
            order.StatusID = (int)OrderStatus.Approved;

            var Email = order.GetMail();
            Assert.AreEqual("Order Processed.", Email.Subject);
        }
    }
}
