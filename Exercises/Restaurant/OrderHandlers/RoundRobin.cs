using System.Collections.Generic;

namespace Restaurant.OrderHandlers
{
    //public class RoundRobin : IHandle<T>
    //{
    //    private readonly Queue<IHandle<T>> handlers;

    //    public RoundRobin(IEnumerable<IHandle<T> handlers)
    //    {
    //        this.handlers = new Queue<IHandle>(handlers);
    //    }

    //    public void HandleOrder(Order order)
    //    {
    //        var handler = handlers.Dequeue();

    //        try
    //        {
    //            handler.Handle(order);
    //        }
    //        finally 
    //        {
    //            handlers.Enqueue(handler);
    //        }
    //    }
    //}
}
