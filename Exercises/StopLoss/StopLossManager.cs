using System.Collections.Generic;
using System.Linq;

namespace StopLoss
{
    public class StopLossManager
    {
        private const decimal MoveStopLossThreshold = 10.0M;
        private const decimal TriggerStopLossThreshold = 7.0M;

        private readonly IMessageBus _bus;
        private readonly List<decimal> priceUpdates = new List<decimal>();

        private decimal? boughtAt;
        private decimal currentStopTarget;

        public StopLossManager(IMessageBus bus)
        {
            _bus = bus;
        }

        public void Consume(PositionAcquired positionAcquired)
        {
            boughtAt = positionAcquired.Price;  
            RaiseTargetUpdated(positionAcquired.Price);
        }

        public void Consume(PriceUpdated priceUpdated)
        {
            if (!boughtAt.HasValue)
                return;

            priceUpdates.Add(priceUpdated.Price);

            _bus.Publish(new SendToMeInX<PriceUpdated>(MoveStopLossThreshold, priceUpdated));
            _bus.Publish(new SendToMeInX<PriceUpdated>(TriggerStopLossThreshold, priceUpdated));
        }

        public void Consume(SendToMeInX<PriceUpdated> msg)
        {
            if (!boughtAt.HasValue)
                return;

            var minPrice = priceUpdates.Any() ? priceUpdates.Min() : boughtAt.Value;
            if (minPrice > currentStopTarget)
            {
                RaiseTargetUpdated(minPrice);   
            }

            var maxPrice = priceUpdates.Any() ? priceUpdates.Max() : boughtAt.Value;
            if (maxPrice < currentStopTarget)
            {
                boughtAt = null;
                priceUpdates.Clear();
                _bus.Publish(new StopLossTriggered());
            }
            priceUpdates.Remove(msg.Message.Price);
        }

        private void RaiseTargetUpdated(decimal price)
        {
            currentStopTarget = price - StopLossThreshold;
            var targetUpdated = new TargetUpdated(currentStopTarget);
            _bus.Publish(targetUpdated); 
        }

        public const decimal StopLossThreshold = 0.1m;
    }
}