using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Restaurant.Tests
{
    [TestFixture]
    public class OrderTests
    {
        [Test]
        public void Serialise()
        {
            var order = new Order
            {
                OrderId = "1",
                TableNumber = 5,
                ServerName = "Dave",
                TimeStamp = "12:00",
                TimeToCook = "00:00",
                Subtotal = 5.55M,
                Total = 6.66M,
                Tax = 1.11M,
                Ingredients = new[] {"Pasta", "Fish"},
                Paid = true,
                Items = new[]
                {
                    new OrderItem {ItemName = "5", Qty = 2, Price = 5.00M},
                    new OrderItem {ItemName = "6", Qty = 3, Price = 6.00M},
                }
            };

            var json = order.Serialize();

            Assert.That(json.GetValue("OrderId").ToString(), Is.EqualTo("1"));
            Assert.That(json.GetValue("TableNumber").ToObject<int>(), Is.EqualTo(5));
            Assert.That(json.GetValue("ServerName").ToString(), Is.EqualTo("Dave"));
            Assert.That(json.GetValue("TimeStamp").ToString(), Is.EqualTo("12:00"));
            Assert.That(json.GetValue("TimeToCook").ToString(), Is.EqualTo("00:00"));
            Assert.That(json.GetValue("Subtotal").ToObject<decimal>(), Is.EqualTo(5.55M));
            Assert.That(json.GetValue("Total").ToObject<decimal>(), Is.EqualTo(6.66M));
            Assert.That(json.GetValue("Tax").ToObject<decimal>(), Is.EqualTo(1.11M));
            Assert.That(json.GetValue("Paid").ToObject<bool>(), Is.EqualTo(true));

            var ingredients = json.GetValue("Ingredients");
            Assert.That(ingredients[0].ToString(), Is.EqualTo("Pasta"));
            Assert.That(ingredients[1].ToString(), Is.EqualTo("Fish"));

            var items = json.GetValue("Items");
            var item1 = items[0];
            Assert.That(item1["ItemName"].ToObject<string>(), Is.EqualTo("5"));
            Assert.That(item1["Qty"].ToObject<int>(), Is.EqualTo(2));
            Assert.That(item1["Price"].ToObject<decimal>(), Is.EqualTo(5.00M));
            var item2 = items[1];
            Assert.That(item2["ItemName"].ToObject<string>(), Is.EqualTo("6"));
            Assert.That(item2["Qty"].ToObject<int>(), Is.EqualTo(3));
            Assert.That(item2["Price"].ToObject<decimal>(), Is.EqualTo(6.00M));
        }

        [Test]
        public void Deserialise()
        {
            var doc = new JObject
            {
                {"OrderId", "1"},
                {"TableNumber", 5},
                {"ServerName", "Dave"},
                {"TimeStamp", "12:00"},
                {"TimeToCook", "00:00"},
                {"Subtotal", 5.55M},
                {"Total", 6.66M},
                {"Tax", 1.11M},
                {"Ingredients", new JArray(new [] { (object) "Pasta", "Fish"})},
                {"Items", new JArray(new JObject { {"ItemName", 5}, {"Qty", 2}, {"Price", 5M}},new JObject { {"ItemName", 6}, {"Qty", 1}, {"Price", 6M} })},
                {"Paid", true}
            };

            var order = Order.Deserialise(doc.ToString());

            Assert.That(order.OrderId, Is.EqualTo("1"));
            Assert.That(order.TableNumber, Is.EqualTo(5));
            Assert.That(order.ServerName, Is.EqualTo("Dave"));
            Assert.That(order.TimeStamp, Is.EqualTo("12:00"));
            Assert.That(order.TimeToCook, Is.EqualTo("00:00"));
            Assert.That(order.Subtotal, Is.EqualTo(5.55M));
            Assert.That(order.Total, Is.EqualTo(6.66M));
            Assert.That(order.Tax, Is.EqualTo(1.11M));
            Assert.That(order.Paid, Is.True);

            Assert.That(order.Ingredients[0], Is.EqualTo("Pasta"));
            Assert.That(order.Ingredients[1], Is.EqualTo("Fish"));

            var item1 = order.Items[0];
            Assert.That(item1.ItemName, Is.EqualTo("5"));
            Assert.That(item1.Qty, Is.EqualTo(2));
            Assert.That(item1.Price, Is.EqualTo(5.00M));
            var item2 = order.Items[1];
            Assert.That(item2.ItemName, Is.EqualTo("6"));
            Assert.That(item2.Qty, Is.EqualTo(1));
            Assert.That(item2.Price, Is.EqualTo(6.00M));
        }

        [Test]
        public void PreservesRandomCrap()
        {
            var doc = new JObject
            {
                {"OrderId", "1"},
                {"TableNumber", 5},
                {"ServerName", "Dave"},
                {"TimeStamp", "12:00"},
                {"TimeToCook", "00:00"},
                {"Subtotal", 5.55M},
                {"Total", 6.66M},
                {"Tax", 1.11M},
                {"Ingredients", new JArray(new [] { (object) "Pasta", "Fish"})},
                {"Items", new JArray(new JObject { {"ItemName", 5}, {"Qty", 2}, {"Price", 5M}},new JObject { {"ItemName", 6}, {"Qty", 1}, {"Price", 6M} })},
                {"Random Crap", "Tear in page"},
                {"Paid", true}
            };

            var order = Order.Deserialise(doc.ToString());
            var json = order.Serialize();

            Assert.That(json.GetValue("Random Crap").ToObject<string>(), Is.EqualTo("Tear in page"));
        }
    }
}
