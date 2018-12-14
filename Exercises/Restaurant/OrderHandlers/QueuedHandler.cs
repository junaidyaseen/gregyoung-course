using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Restaurant.OrderHandlers
{
    public class QueuedHandler<T> : IHandle<T>, IStartable
    {
        private readonly ConcurrentQueue<T> workQueue = new ConcurrentQueue<T>();
        private readonly IHandle<T> handler;
        private readonly Thread workerThread;

        public QueuedHandler(string name, IHandle<T> handler)
        {
            this.handler = handler;

            workerThread = new Thread(OrderHandler) { Name = name };
        }

        public decimal QueueCount { get { return workQueue.Count;  } }

        private void OrderHandler()
        {
            while (true)
            {
                T message;
                if (workQueue.TryDequeue(out message))
                    handler.Handle(message);
                else
                    Thread.Sleep(1);
            }
        }

        public void Handle(T message)
        {
            workQueue.Enqueue(message);
        }

        public void Start()
        {
            workerThread.Start();
        }

        public string GetStatistics()
        {
            return string.Format("{0} queue count {1}", workerThread.Name, workQueue.Count);
        }       
    }
}
