using Microsoft.EntityFrameworkCore;
using Outbox.Common;

namespace Outbox.Order.Data;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options) : base(options)
    { }
    
    public DbSet<Models.Order> Orders => Set<Models.Order>();
    public DbSet<OutboxEventEntity> OutboxEntity => Set<OutboxEventEntity>();
}