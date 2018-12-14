using System;
using System.Linq;
using NUnit.Framework;

namespace Restaurant.Tests
{
    [TestFixture]
    public class MidgetTest
    {
        private readonly Guid orderGuid = Guid.NewGuid();
        readonly Order order = new Order
        {
            OrderId = "1",
            TableNumber = 5,
            ServerName = "Dave",
            TimeStamp = "12:00",
            TimeToCook = "00:00",
            Subtotal = 5.55M,
            Total = 6.66M,
            Tax = 1.11M,
            Ingredients = new[] { "Pasta", "Fish" },
            Paid = true,
            Items = new[]
                {
                    new OrderItem {ItemName = "5", Qty = 2, Price = 5.00M},
                    new OrderItem {ItemName = "6", Qty = 3, Price = 6.00M},
                }
        };

        private FakeBus bus;

        [SetUp]
        public void SetUp()
        {
            bus = new FakeBus();
        }

        [Test]
        public void WhenMidgetNotifiedOfOrderPlaced_ThenMidgetSendsCommandToCookFood()
        {
           
            var orderPlaced = new OrderPlaced(order, Guid.NewGuid(), orderGuid);

            var midget = new EnglishMidget(bus, orderGuid);
            midget.Handle(orderPlaced);

            //var cookFood = (CookFood) bus.Messages.Single();
            Assert.That(bus.Messages.Single(), Is.InstanceOf<CookFood>());
        }

        [Test]
        public void WhenMidgetNotifiedOfFoodCooked_ThenMidgetSendsCommandToPriceOrder()
        {
            var orderCooked = new OrderCooked(order, Guid.NewGuid(), orderGuid);

            var midget = new EnglishMidget(bus, orderGuid);
            midget.Handle(orderCooked);

            //var cookFood = (CookFood) bus.Messages.Single();
            Assert.That(bus.Messages.Single(), Is.InstanceOf<PriceOrder>());
        }


        [Test]
        public void WhenMidgetNotifiedOfOrderPriced_ThenMidgetSendsCommandToTakePayment()
        {
            var orderPriced = new OrderPriced(order, Guid.NewGuid(), orderGuid);

            var midget = new EnglishMidget(bus, orderGuid);
            midget.Handle(orderPriced);

            Assert.That(bus.Messages.Single(), Is.InstanceOf<TakePayment>());
        }
    }
}
