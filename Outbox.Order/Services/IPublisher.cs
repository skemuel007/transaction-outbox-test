using Outbox.Common;

namespace Outbox.Order.Services;

public interface IPublisher
{
    void Publish(OutboxEventEntity outboxEventEntity);
}