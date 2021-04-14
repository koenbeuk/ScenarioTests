using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ScenarioTests.Internal
{
    public sealed class ScenarioFactTestCase : XunitTestCase
    {
        string _factName;
        bool _isTheory;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public ScenarioFactTestCase() { }

        public ScenarioFactTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, TestMethodDisplayOptions defaultMethodDisplayOptions, ITestMethod testMethod, object[] testMethodArguments = null, string factName = null, SourceInformation sourceInformation = null, bool isTheory = false) 
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
        {
            _factName = factName;
            _isTheory = isTheory;
            SourceInformation = sourceInformation;
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            data.AddValue("FactName", _factName);
            data.AddValue("IsTheory", _isTheory);

            base.Serialize(data);
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            _factName = data.GetValue<string>("FactName");
            _isTheory = data.GetValue<bool>("IsTheory");

            base.Deserialize(data);
        }

        public string FactName => _factName;

        public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) 
        {
            return new ScenarioFactTestCaseRunner(this, DisplayName, SkipReason, constructorArguments, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource).RunAsync();
        }
    }
}
