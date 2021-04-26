using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ScenarioTests.Internal
{
    sealed internal class BufferedMessageBus : IMessageBus
    {
        readonly IMessageBus _messageBus;
        readonly List<IMessageSinkMessage> _queue = new();

        public BufferedMessageBus(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public bool QueueMessage(IMessageSinkMessage message)
        {
            _queue.Add(message);

            return true; // prevent xunit from cancelling
        }

        public IEnumerable<IMessageSinkMessage> QueuedMessages => _queue;

        public void Dispose() { }
    }
}
