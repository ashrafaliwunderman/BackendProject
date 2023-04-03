using Core.PublishService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MessageQueuePubService.ClosePeddingOrderService
{
    public class PushOrderService : IPushOrderService
    {
        private readonly ConnectionFactory _factory;

        public PushOrderService()
        {
            _factory = new ConnectionFactory();
        }

        public Task CloseOrderRequestPublisher(int id)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("closer-order-id", exclusive: false);

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(id));

            channel.BasicPublish(exchange: "",
                                 routingKey: "closer-order-id",
                                 basicProperties: null,
                                 body: body);
            return Task.CompletedTask;
        }
    }
}
