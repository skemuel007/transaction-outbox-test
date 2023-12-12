using System.Text;
using System.Text.Json;
using Outbox.Common;
using Outbox.Notification.Data;
using Outbox.Notification.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Outbox.Notification.Services;

public class EventsListener: BackgroundService
    {
        private readonly IModel _channel;
        private readonly ILogger<EventsListener> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventsListener(ILogger<EventsListener> logger, IServiceScopeFactory serviceScopeFactory)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "OrderEventsQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            NotificationContext? notificationDb = scope.ServiceProvider.GetService<NotificationContext>();
            
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation($"Received message: {message}");

                var outboxEventEntity = JsonSerializer.Deserialize<OutboxEventEntity>(message);
                if(outboxEventEntity != null)
                {
                    var orderEvent = JsonSerializer.Deserialize<OrderEvent>(outboxEventEntity.Data);
                    if (orderEvent != null)
                    {
                        notificationDb?.Notifications.Add(new Models.Notification 
                        {
                            Id = Guid.NewGuid(),
                            OrderId = orderEvent.OrderId,
                            CustomerId = orderEvent.CustomerId,
                            ResturantId = orderEvent.RestaurantId,
                            //OrderItems = orderEvent.OrderItems,
                            NotificationType = NotificationType.NotifyToResturant
                        });

                        _logger.LogInformation($"Notifying to Restaurant about OrderId: {orderEvent.OrderId}");

                        await notificationDb.SaveChangesAsync(stoppingToken);

                        notificationDb.Notifications.Add(new Models.Notification
                        {
                            Id = Guid.NewGuid(),
                            CustomerId = orderEvent.CustomerId,
                            ResturantId = orderEvent.RestaurantId,
                            //OrderItems = orderEvent.OrderItems,
                            NotificationType = NotificationType.NotifyToDeliveryAgent
                        });

                        _logger.LogInformation($"Notifying to Delivery Agent about OrderId: {orderEvent.OrderId}");

                        await notificationDb.SaveChangesAsync(stoppingToken);
                    }    
                }
                
            };

            _channel.BasicConsume(queue: "OrderEventsQueue", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
    }