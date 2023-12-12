namespace Outbox.Common;

public class OutboxEventEntity
{
    public int Id { get; set; }
    public string Event { get; set; }
    public string Data { get; set; }
}