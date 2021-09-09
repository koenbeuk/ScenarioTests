using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ScenarioTests.Internal
{

    [Serializable]
    public class ScenarioAbortException : Exception
    {
        public ScenarioAbortException() { }
        public ScenarioAbortException(string message) : base(message) { }
        public ScenarioAbortException(string message, Exception inner) : base(message, inner) { }
        protected ScenarioAbortException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
