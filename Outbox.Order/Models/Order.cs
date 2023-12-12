namespace Outbox.Order.Models;

public class Order
{
    public Guid Id { get; set; }
    public int CustomerId { get; set; }
    public int ResturantId { get; set; }
    public DateTime OrderDate { get; set; }
    public double OrderAmount { get; set; }


    public List<OrderItem> OrderItems { get; set; }
}