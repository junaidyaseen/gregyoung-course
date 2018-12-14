using System;

namespace Restaurant.OrderHandlers
{
    public class TimeToLiveDispatcher<T> : IHandle<T> where T : IMessage
    {
        private readonly IHandle<T> handler;

        public TimeToLiveDispatcher(IHandle<T> handler)
        {
            this.handler = handler;
        }

        public void Handle(T message)
        {
            if (!message.TimeToLive.HasValue || message.TimeToLive > DateTime.UtcNow)
            {
                handler.Handle(message);
            }
            else
            {
                Console.WriteLine("Message past TTL, discarding. {0}", message.MessageId);
            }
        }
    }
}
