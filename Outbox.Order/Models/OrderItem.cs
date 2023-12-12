namespace Outbox.Order.Models;

public class OrderItem
{
    public Guid Id { get; set; }
    public int ItemId { get; set; } 
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }


    public Guid? OrderId { get; set; }
    //public Order Order { get; set; }
}