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
    sealed internal class ScenarioFactTestCase : XunitTestCase
    {
        string _factName;
        int _theoryTestCaseLimit;
        ScenarioTestExecutionPolicy _scenarioTestExecutionPolicy;
        bool _runInIsolation;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public ScenarioFactTestCase() { }

        public ScenarioFactTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, TestMethodDisplayOptions defaultMethodDisplayOptions, ITestMethod testMethod, object[] testMethodArguments, string factName, SourceInformation sourceInformation, int theoryTestCaseLimit, ScenarioTestExecutionPolicy scenarioTestExecutionPolicy, bool runInIsolation) 
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
        {
            _factName = factName;
            _theoryTestCaseLimit = theoryTestCaseLimit;
            _scenarioTestExecutionPolicy = scenarioTestExecutionPolicy;
            _runInIsolation = runInIsolation;

            SourceInformation = sourceInformation;
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            data.AddValue("FactName", _factName);
            data.AddValue("TheoryTestCaseLimits", _theoryTestCaseLimit);
            data.AddValue("ExecutionPolicy", _scenarioTestExecutionPolicy);
            data.AddValue("RunInIsolation", _runInIsolation);

            base.Serialize(data);
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            _factName = data.GetValue<string>("FactName");
            _theoryTestCaseLimit = data.GetValue<int>("TheoryTestCaseLimits");
            _scenarioTestExecutionPolicy = data.GetValue<ScenarioTestExecutionPolicy>("ExecutionPolicy");
            _runInIsolation = data.GetValue<bool>("RunInIsolation");

            base.Deserialize(data);
        }

        public string FactName => _factName;
        public int TheoryTestCaseLimit => _theoryTestCaseLimit;
        public ScenarioTestExecutionPolicy ExecutionPolicy => _scenarioTestExecutionPolicy;
        public bool RunInIsolation => _runInIsolation;

        public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource) 
            => new ScenarioFactTestCaseRunner(this, DisplayName, SkipReason, constructorArguments, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource).RunAsync();
    }
}
