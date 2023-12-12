namespace Outbox.Common;

public class OrderEvent
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public int RestaurantId { get; set; }
    public double OrderAmount { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}