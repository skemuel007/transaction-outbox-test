using Outbox.Order.Data;

namespace Outbox.Order.Services;

public class OutboxPublisher : BackgroundService
{
    private readonly ILogger<OutboxPublisher> _logger;
    private readonly IPublisher _publisher;
    private readonly OrderContext _orderDb;

    public OutboxPublisher(ILogger<OutboxPublisher> logger, IPublisher publisher, OrderContext orderDb)
    {
        _logger = logger;
        _publisher = publisher;
        _orderDb = orderDb;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var outboxEvents = _orderDb.OutboxEntity.OrderBy(o => o.Id).ToList();
                foreach (var outboxEvent in outboxEvents)
                {
                    _publisher.Publish(outboxEvent);
                    _orderDb.OutboxEntity.Remove(outboxEvent);
                    await _orderDb.SaveChangesAsync();
                } 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in publishing outboxevnt: {ex.Message}");
            }
            finally
            {
                await Task.Delay(2500, stoppingToken);
            }
        }
    }
}