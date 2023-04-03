using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClosePeddingReqScheduler
{
    public class ClosePeddingOrderSubcriber : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public ClosePeddingOrderSubcriber(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var _factory = new ConnectionFactory();
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare("closer-order-id", exclusive: false);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received +=  (model, e) =>
            {
                var body = e.Body;
                var json = System.Text.Encoding.UTF8.GetString(body.ToArray());
                var id = JsonSerializer.Deserialize<int>(json);
                SendClosePeddingRequestByID(id);
            };
            channel.BasicConsume("closer-order-id", true, consumer);
        }


        private async Task SendClosePeddingRequestByID(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri("https://localhost:44337");
                    //var content = new FormUrlEncodedContent(new[]
                    //{
                    //    new KeyValuePair<string, string>("id", id.ToString())
                    //});


                    //var result = await client.PostAsync("/api/Order/CloseOrder/", content);
                    //string resultContent = await result.Content.ReadAsStringAsync();
                    //Console.WriteLine(resultContent);

                    //using var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44337/api/Order/CloseOrder/" + id.ToString());
                    //request.Content = JsonContent.Create(new { id = id});
                    var response = await client.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request exception: {e.Message}");
            }
        }
    }
}
