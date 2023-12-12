namespace Outbox.Order.Dtos;

public class OrderItemDTO
{
    public int ItemId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }
}