using BasketApp.Core.Domain.BasketAggregate;
using BasketApp.Core.Ports;
using DiscountApp.Api;
using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;

namespace BasketApp.Infrastructure.Adapters.Grpc.DiscountService;

public class Client : IDiscountClient
{
    private readonly string _url;
    private readonly SocketsHttpHandler _socketsHttpHandler;
    private readonly MethodConfig _methodConfig;

    public Client(string url)
    {
        _url = url;

        _socketsHttpHandler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true
        };

        _methodConfig = new MethodConfig
        {
            Names = {MethodName.Default},
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = {StatusCode.Unavailable}
            }
        };
    }

    public async Task<double> GetDiscountAsync(Basket basket, CancellationToken cancellationToken)
    {
        using var channel = GrpcChannel.ForAddress(_url, new GrpcChannelOptions
        {
            HttpHandler = _socketsHttpHandler,
            ServiceConfig = new ServiceConfig {MethodConfigs = {_methodConfig}}
        });

        var client = new Discount.DiscountClient(channel);
        try
        {
            var items = new RepeatedField<DiscountApp.Api.Item>();
            foreach (var basketItems in basket.Items)
            {
                var item = new DiscountApp.Api.Item()
                {
                    Id = basketItems.Id.ToString()
                };
                items.Add(item);
            }

            var reply = await client.GetDiscountAsync(new GetDiscountRequest
            {
                Items = {items}
            }, null, deadline: DateTime.UtcNow.AddSeconds(5), cancellationToken);
            return reply.Value;
        }
        catch (RpcException)
        {
            //Fallback
            return 0.05;
        }
    }
}