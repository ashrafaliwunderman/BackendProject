using ClosePeddingReqScheduler;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddHostedService<ClosePeddingOrderSubcriber>();
    })
    .Build();

await host.RunAsync();
