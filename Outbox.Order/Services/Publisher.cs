using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Outbox.Common;
using RabbitMQ.Client;

namespace Outbox.Order.Services;

public class Publisher : IPublisher
{
    private readonly ILogger<Publisher> _logger;
    private readonly IModel _channel;

    public Publisher(ILogger<Publisher> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "OrderEventsQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }
    
    public void Publish(OutboxEventEntity outboxEventEntity)
    {
        string message = JsonSerializer.Serialize(outboxEventEntity);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "",
            routingKey: "OrderEventsQueue",
            basicProperties: null,
            body: body);
        _logger.LogInformation("Event pushed to orderEventsQueue........");
    }
}