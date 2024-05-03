using BasketApp.Core.Domain.BasketAggregate.DomainEvents;
using BasketApp.Core.Ports;
using BasketConfirmed;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace BasketApp.Infrastructure.Adapters.Kafka.BasketConfirmed;

/// <summary>
///  Producer для Kafka
/// </summary>
public sealed class Producer : IBusProducer
{
    private readonly ProducerConfig _config;
    private readonly string _topicName = "basket.confirmed";

    public Producer(string messageBrokerHost)
    {
        if (string.IsNullOrWhiteSpace(messageBrokerHost)) throw new ArgumentException(nameof(messageBrokerHost));
        _config = new ProducerConfig
        {
            BootstrapServers = messageBrokerHost
        };
    }

    public async Task Publish(BasketConfirmedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Перекладываем данные из Domain Event в Integration Event
        var basketConfirmedIntegrationEvent = new BasketConfirmedIntegrationEvent
        {
            BasketId = notification.BasketId.ToString(),
            Address = notification.Address,
            Weight = notification.Weight
        };

        // Создаем сообщение для Kafka
        var message = new Message<string, string>
        {
            Key = notification.EventId.ToString(),
            Value = JsonConvert.SerializeObject(basketConfirmedIntegrationEvent)
        };
        
        // Отправляем сообщение в Kafka
        using var producer = new ProducerBuilder<string, string>(_config).Build();
        await producer.ProduceAsync(_topicName, message, cancellationToken);
    }
}