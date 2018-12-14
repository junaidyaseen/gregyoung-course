using System.Collections.Generic;

namespace Restaurant.OrderHandlers
{
    public class AssistantManager : IHandle<PriceOrder>
    {
        private readonly ITopicBasedPubSub bus;
        private const decimal TaxRate = 0.2M;

        private readonly Dictionary<string, decimal> dishPrices = new Dictionary<string, decimal>
        {
            { "Spaghetti Bolognese", 27.90M },
            { "Fish", 23.90M }
        }; 

        public AssistantManager(ITopicBasedPubSub bus)
        {
            this.bus = bus;
        }

        public void Handle(PriceOrder message)
        {
            var order = message.Order;
            decimal subtotal = 0M;
            foreach (var item in order.Items)
            {
                var price = dishPrices[item.ItemName];
                item.Price = price;
                subtotal += price * item.Qty;
            }

            var tax = subtotal * TaxRate;

            order.Subtotal = subtotal;
            order.Tax = tax;
            order.Total = subtotal + tax;

            bus.Publish(new OrderPriced(order, message.MessageId, message.CorrelationId));
        }
    }
}
