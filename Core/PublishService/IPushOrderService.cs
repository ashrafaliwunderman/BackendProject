using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.PublishService
{
    public interface IPushOrderService
    {
        Task CloseOrderRequestPublisher(int id);
    }
}
