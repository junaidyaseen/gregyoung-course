using System.Linq;
using NUnit.Framework;

namespace StopLoss.Tests
{
    [TestFixture]
    public class StopLossTests
    {
        private FakeMessageBus bus;
        private StopLossManager stopLossManager;

        [SetUp]
        public void SetUp()
        {
            bus = new FakeMessageBus();
            stopLossManager = new StopLossManager(bus);   
        }

        [Test]
        public void WhenPositionAcquiredThenTargetUpdated()
        {
            var  positionAcquired = new PositionAcquired(1.0M);
            stopLossManager.Consume(positionAcquired);

            var events = bus.GetMessages();
            var targetUpdated = events.First() as TargetUpdated;
            Assert.That(targetUpdated, Is.Not.Null, "Should be TargetUpdated");
            Assert.That(targetUpdated.TargetPrice, Is.EqualTo(0.9M));
        }

        [Test]
        public void WhenPriceUpdatedWithinTenSecondsThenRaisesTwoSendInX()
        {
            var positionAcquired = new PositionAcquired(1.0M);
            stopLossManager.Consume(positionAcquired);

            var priceUpdated = new PriceUpdated {Price = 1.5M};
            stopLossManager.Consume(priceUpdated);

            var events = bus.GetMessages().OfType<SendToMeInX<PriceUpdated>>().ToArray();
            Assert.That(events, Has.Length.EqualTo(2));
            var sendToMeInX1 = events.First();
            var sendToMeInX2 = events.Last();


            Assert.That(sendToMeInX1, Is.Not.Null);
            Assert.That(sendToMeInX1.Message, Is.EqualTo(priceUpdated));
            Assert.That(sendToMeInX1.X, Is.EqualTo(10.0M));

            Assert.That(sendToMeInX2, Is.Not.Null);
            Assert.That(sendToMeInX2.Message, Is.EqualTo(priceUpdated));
            Assert.That(sendToMeInX2.X, Is.EqualTo(7.0M));
        }

        [Test]
        public void WhenPriceSustainedForLongerThan10SecondsThenTargetUpdated()
        {
            var positionAcquired = new PositionAcquired(1.0M);
            stopLossManager.Consume(positionAcquired);

            var priceUpdated = new PriceUpdated { Price = 1.5M };
            stopLossManager.Consume(priceUpdated);
            var sendToMeInX = new SendToMeInX<PriceUpdated>(10.0M, priceUpdated);
            stopLossManager.Consume(sendToMeInX);

            var targetUpdated = bus.GetLastMessage<TargetUpdated>();

            Assert.That(targetUpdated, Is.Not.Null);
            Assert.That(targetUpdated.TargetPrice, Is.EqualTo(1.4M));
        }

        [Test]
        public void WhenPriceNotSustainedForLongerThan10SecondsThenTargetNotUpdated()
        {
            var positionAcquired = new PositionAcquired(1.0M);
            stopLossManager.Consume(positionAcquired);

            var priceUpdated = new PriceUpdated { Price = 0.8M };
            stopLossManager.Consume(priceUpdated);

            bus.Clear();

            priceUpdated = new PriceUpdated { Price = 1.5M };
            var sendToMeInX = new SendToMeInX<PriceUpdated>(10.0M, priceUpdated);
            stopLossManager.Consume(sendToMeInX);

            var targetUpdated = bus.GetLastMessage<TargetUpdated>();

            Assert.That(targetUpdated, Is.Null);
        }

        [Test]
        public void WhenPriceGoesUpAndThenDownThenTakesTheMinimumSustainedIncrease()
        {
            var positionAcquired = new PositionAcquired(1.0M);
            stopLossManager.Consume(positionAcquired);

            var priceUpdated = new PriceUpdated { Price = 1.5M };
            stopLossManager.Consume(priceUpdated);

            priceUpdated = new PriceUpdated { Price = 1.2M };
            stopLossManager.Consume(priceUpdated);

            bus.Clear();

            priceUpdated = new PriceUpdated { Price = 1.5M };
            var sendToMeInX = new SendToMeInX<PriceUpdated>(10.0M, priceUpdated);
            stopLossManager.Consume(sendToMeInX);

            var targetUpdated = bus.GetLastMessage<TargetUpdated>();

            Assert.That(targetUpdated, Is.Not.Null);
            Assert.That(targetUpdated.TargetPrice, Is.EqualTo(1.1M));
        }

        [Test]
        public void WhenPriceGoesDownAndIsSustainedForLongerThan7SecondsThenTriggerStopLoss()
        {
            var positionAcquired = new PositionAcquired(1.0M);
            stopLossManager.Consume(positionAcquired);

            var priceUpdated = new PriceUpdated { Price = 0.89M };
            stopLossManager.Consume(priceUpdated);

            var sendToMeInX = new SendToMeInX<PriceUpdated>(7.0M, priceUpdated);
            stopLossManager.Consume(sendToMeInX);

            var stopLossTriggered = bus.GetLastMessage<StopLossTriggered>();

            Assert.That(stopLossTriggered, Is.Not.Null);
        }

        [Test]
        public void WhenPriceGoesDownAndIsNotSustainedForLongerThan7SecondsThenDoesntTriggerStopLoss()
        {
            var positionAcquired = new PositionAcquired(1.0M);
            stopLossManager.Consume(positionAcquired);

            var priceUpdated = new PriceUpdated { Price = 0.89M };
            stopLossManager.Consume(priceUpdated);

            var priceUpdated2 = new PriceUpdated { Price = 0.91M };
            stopLossManager.Consume(priceUpdated2);

            var sendToMeInX = new SendToMeInX<PriceUpdated>(7.0M, priceUpdated);
            stopLossManager.Consume(sendToMeInX);

            var stopLossTriggered = bus.GetLastMessage<StopLossTriggered>();

            Assert.That(stopLossTriggered, Is.Null);
        }


        [Test]
        public void WhenPriceUpdatedAfterThresholdThenTargetUpdatedWithNewPriceWithoutObsoletePricesAffectingIt()
        {
            var positionAcquired = new PositionAcquired(1.0M);
            stopLossManager.Consume(positionAcquired);

            var priceUpdated = new PriceUpdated { Price = 1.0M };
            stopLossManager.Consume(priceUpdated);

            var priceUpdated2 = new PriceUpdated { Price = 1.2M };
            stopLossManager.Consume(priceUpdated2);

            bus.Clear();

            stopLossManager.Consume(new SendToMeInX<PriceUpdated>(10.0M, priceUpdated));
            stopLossManager.Consume(new SendToMeInX<PriceUpdated>(10.0M, priceUpdated2));

            var targetUpdated = bus.GetLastMessage<TargetUpdated>();

            Assert.That(targetUpdated, Is.Not.Null);
            Assert.That(targetUpdated.TargetPrice, Is.EqualTo(1.1M));
        }

        [Test]
        public void AfterSellingDoesntSellAgain()
        {
            var positionAcquired = new PositionAcquired(1.0M);
            stopLossManager.Consume(positionAcquired);

            var priceUpdated = new PriceUpdated { Price = 0.89M };
            stopLossManager.Consume(priceUpdated);

            var priceUpdated2 = new PriceUpdated { Price = 0.5M };
            stopLossManager.Consume(priceUpdated);

            stopLossManager.Consume(new SendToMeInX<PriceUpdated>(7.0M, priceUpdated));

            bus.Clear();
            stopLossManager.Consume(new SendToMeInX<PriceUpdated>(7.0M, priceUpdated2));

            var stopLossTriggered = bus.GetLastMessage<StopLossTriggered>();

            Assert.That(stopLossTriggered, Is.Null);
        }
    }
}
