using Outbox.Order.Data;

namespace Outbox.Order.Services;

public class OutboxPublisher : BackgroundService
{
    private readonly ILogger<OutboxPublisher> _logger;
    private readonly IPublisher _publisher;
    // private readonly OrderContext _orderDb;
    private readonly IServiceScopeFactory _scopeFactory;

    public OutboxPublisher(ILogger<OutboxPublisher> logger, IPublisher publisher, 
        IServiceScopeFactory scopeFactory/*OrderContext orderDb*/)
    {
        _logger = logger;
        _publisher = publisher;
        // _orderDb = orderDb;
        _scopeFactory = scopeFactory;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // https://stackoverflow.com/questions/75017055/how-to-add-background-service-which-uses-dbcontecxt-in-asp-net-core-web-api-proj
        using IServiceScope scope = _scopeFactory.CreateScope();
        OrderContext _orderDb = scope.ServiceProvider.GetService<OrderContext>();
        
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