using Core.Model.Email;
using Core.PublishService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace MessageQueuePubService.EmailRequestService
{
    public class EmailQueueService : IEmailQueueService
    {
        private readonly ConnectionFactory _factory;

        public EmailQueueService()
        {
            _factory = new ConnectionFactory();
        }

        public Task PushQueue(Email email)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("email-send", exclusive: false);

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(email));

            channel.BasicPublish(exchange: "",
                                 routingKey: "email-send",
                                 basicProperties: null,
                                 body: body);

            return Task.CompletedTask;

        }
    }
}