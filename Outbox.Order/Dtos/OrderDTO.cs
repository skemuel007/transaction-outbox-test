namespace Outbox.Order.Dtos;

public class OrderDTO
{
    public int ResturantId { get; set; }
    public List<OrderItemDTO> Items { get; set;}
}