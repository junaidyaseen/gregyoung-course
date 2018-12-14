using System;

namespace Restaurant
{
    public interface IMessage
    {
        Guid MessageId { get; }
        Guid CausationId { get; }
        Guid CorrelationId { get; }
        DateTime? TimeToLive { get; }
    }

    public class BaseEvent : IMessage
    {
        public BaseEvent(Guid causationId, Guid correlationId, DateTime? timeToLive = null)
        {
            CausationId = causationId;
            CorrelationId = correlationId;
            TimeToLive = timeToLive;
            MessageId = Guid.NewGuid();
        }

        public Guid MessageId { get; private set; }
        public Guid CausationId { get; private set; }
        public Guid CorrelationId { get; private set; }
        public DateTime? TimeToLive { get; set; }
    }

    public class OrderPlaced : BaseEvent
    {
        public OrderPlaced(Order order, Guid causationId, Guid correlationId, DateTime? timeToLive = null)
            : base(causationId, correlationId, timeToLive)
        {
            Order = order;
        }
        public Order Order { get; private set; }
    }

    public class OrderCooked : BaseEvent
    {
        public OrderCooked(Order order, Guid causationId, Guid correlationId, DateTime? timeToLive = null)
            : base(causationId, correlationId, timeToLive)
        {
            Order = order;
        }
        public Order Order { get; private set; }
    }

    public class OrderPriced : BaseEvent
    {
        public OrderPriced(Order order, Guid causationId, Guid correlationId, DateTime? timeToLive = null)
            : base(causationId, correlationId, timeToLive)
        {
            Order = order;
        }

        public Order Order { get; private set; }
    }

    public class OrderPaid : BaseEvent
    {
        public OrderPaid(Order order, Guid causationId, Guid correlationId, DateTime? timeToLive = null)
            : base(causationId, correlationId, timeToLive)
        {
            Order = order;
        }

        public Order Order { get; private set; }
    }

    public class OrderComplete : BaseEvent
    {
        public OrderComplete(Order order, Guid causationId, Guid correlationId, DateTime? timeToLive = null)
            : base(causationId, correlationId, timeToLive)
        {
            Order = order;
        }

        public Order Order { get; private set; }
    }

}
