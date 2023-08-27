using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotApi.Game.Extensions
{
    public static class QueueExtensions
    {
        public static void EnqueueRange<T>(this ConcurrentQueue<T> queue, IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                queue.Enqueue(item);
            }
        }
    }
}
