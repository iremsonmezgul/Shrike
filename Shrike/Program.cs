using Shrike.Logging;
using StackExchange.Redis;
using Shrike.Services;
using Shrike.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var redis = ConnectionMultiplexer.Connect("localhost:6379");
var cacheService = new RedisCacheService(redis);
var loadBalancer = new LoadBalancer(cacheService);
var udpLogger = new UdpLogger(loadBalancer);

udpLogger.StartListening();

var timer = new System.Timers.Timer(60000);
timer.Elapsed += async (sender, e) =>
{
    await loadBalancer.RefreshHummingbirds();
};
timer.Start();

app.MapPost("/register", async (Hummingbird hummingbird) =>
{
    await cacheService.RegisterHummingbird(hummingbird);
    return Results.Ok("Registered");
});

app.MapGet("/redirect", async () =>
{
    var hummingbird = await loadBalancer.GetRedirectHummingbird();
    return hummingbird is not null ? Results.Ok(hummingbird) : Results.NotFound("No available Hummingbird");
});

app.Run();

