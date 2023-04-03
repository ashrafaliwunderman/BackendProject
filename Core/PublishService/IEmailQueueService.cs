using Core.Model.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.PublishService
{
    public interface IEmailQueueService
    {
        Task PushQueue(Email email);

    }
}
