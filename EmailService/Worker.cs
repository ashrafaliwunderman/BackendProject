using Core.Model.Email;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace EmailService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var _factory = new ConnectionFactory();
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare("email-send", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, e) =>
            {
                var body = e.Body;
                var json = System.Text.Encoding.UTF8.GetString(body.ToArray());
                var email = JsonSerializer.Deserialize<Email>(json);

                IEmailSender emailSender = new EmailSender();
                emailSender.SendEmailAsync(email);

            };
            channel.BasicConsume("email-send", true, consumer);
        }
    }
}