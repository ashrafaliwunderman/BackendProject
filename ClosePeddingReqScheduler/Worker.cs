namespace ClosePeddingReqScheduler
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
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44337/api/Order/closePeddingOrder");
                    var response = await client.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(content);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }


                await Task.Delay(1000*120, stoppingToken);
            }
        }
    }
}