using System;

namespace Restaurant
{
    public class CookFood : BaseEvent
    {
        public CookFood(Order order, Guid causationId, Guid correlationId, DateTime? timeToLive = null)
            : base (causationId, correlationId, timeToLive)
        {
            Order = order;
        }

        public Order Order { get; set; }
    }

    public class PriceOrder : BaseEvent
    {
        public PriceOrder(Order order, Guid causationId, Guid correlationId, DateTime? timeToLive = null)
            : base(causationId, correlationId, timeToLive)
        {
            Order = order;
        }

        public Order Order { get; set; }
    }

    public class TakePayment : BaseEvent
    {
        public TakePayment(Order order, Guid causationId, Guid correlationId, DateTime? timeToLive = null)
            : base(causationId, correlationId, timeToLive)
        {
            Order = order;
        }

        public Order Order { get; set; }
    }
}
