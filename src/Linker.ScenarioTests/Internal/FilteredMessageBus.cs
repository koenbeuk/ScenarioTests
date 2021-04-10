using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Linker.ScenarioTests.Internal
{
    sealed internal class FilteredMessageBus : IMessageBus
    {
        readonly IMessageBus _messageBus;
        readonly Func<IMessageSinkMessage, bool> _filter;

        public FilteredMessageBus(IMessageBus messageBus, Func<IMessageSinkMessage, bool> filter)
        {
            _messageBus = messageBus;
            _filter = filter;
        }

        public bool QueueMessage(IMessageSinkMessage message)
        {
            if (_filter(message))
            {
                return _messageBus.QueueMessage(message);
            }

            return true; // prevent xunit from cancelling
        }

        public void Dispose() { }
    }
}
