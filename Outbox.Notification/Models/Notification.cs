namespace Outbox.Notification.Models;

public class Notification
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public int ResturantId { get; set; }
    //public List<OrderItem> OrderItems { get; set; }
    public NotificationType NotificationType { get; set; }
}