using Microsoft.EntityFrameworkCore;

namespace Outbox.Notification.Data;

public class NotificationContext : DbContext
{
    public NotificationContext(DbContextOptions<NotificationContext> options) : base(options)
    { }

    public DbSet<Models.Notification> Notifications => Set<Models.Notification>();
}