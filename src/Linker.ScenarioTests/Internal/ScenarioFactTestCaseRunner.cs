using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Linker.ScenarioTests.Internal
{
    public class ScenarioFactTestCaseRunner : XunitTestCaseRunner
    {
        static readonly object[] NoArguments = new object[0];

        readonly StringBuilder _parentTestOutputBuilder = new();
        readonly StringBuilder _recordingTestOutputBuilder = new();

        ScenarioContext _scenarioContext;
        bool _isRecording;

        public ScenarioFactTestCaseRunner(IXunitTestCase testCase,
                                         string displayName,
                                         string skipReason,
                                         object[] constructorArguments,
                                         IMessageSink diagnosticMessageSink,
                                         IMessageBus messageBus,
                                         ExceptionAggregator aggregator,
                                         CancellationTokenSource cancellationTokenSource)
            : base(testCase, displayName, skipReason, constructorArguments, NoArguments, messageBus, aggregator, cancellationTokenSource)
        {
            DiagnosticMessageSink = diagnosticMessageSink;
        }

        /// <summary>
        /// Gets the message sink used to report <see cref="IDiagnosticMessage"/> messages.
        /// </summary>
        protected IMessageSink DiagnosticMessageSink { get; }

        protected override async Task<RunSummary> RunTestAsync()
        {
            var scenarioFactTestCase = (ScenarioFactTestCase)TestCase;
            _scenarioContext = new ScenarioContext(scenarioFactTestCase.FactName, RecordTestCase);

            TestMethodArguments = new object[] { _scenarioContext };

            var capturedMessages = new List<IMessageSinkMessage>();
            var filteredMessageBus = new FilteredMessageBus(MessageBus, message =>
            {
                if (message is TestOutput testOutput)
                {
                    if (_isRecording)
                    {
                        _recordingTestOutputBuilder.Append(testOutput.Output);
                    }
                    else
                    {
                        _parentTestOutputBuilder.Append(testOutput.Output);
                    }
                }

                if (message is not ITestMessage)
                {
                    return true;
                }
                else
                {
                    capturedMessages.Add(message);
                    return false;
                }
            });

            _isRecording = false;
            _parentTestOutputBuilder.Clear();
            _recordingTestOutputBuilder.Clear();

            var test = CreateTest(TestCase, DisplayName);
            var result = await CreateTestRunner(test, filteredMessageBus, TestClass, ConstructorArguments, TestMethod, TestMethodArguments, SkipReason, BeforeAfterAttributes, Aggregator, CancellationTokenSource).RunAsync();
            if (result.Failed > 0)
            {
                foreach (var message in capturedMessages)
                {
                    MessageBus.QueueMessage(message);
                }
            }
            
            return result;
            
        }

        async Task RecordTestCase(object[] arguments, Func<Task> invocation)
        {
            string BuildOutput()
            {
                return _parentTestOutputBuilder.ToString() + _recordingTestOutputBuilder.ToString();
            }

            var test = CreateTest(TestCase, DisplayName);
            var stopwatch = new Stopwatch();

            if (!MessageBus.QueueMessage(new TestStarting(test)))
            {
                CancellationTokenSource.Cancel();
            }

            stopwatch.Start();

            try
            {
                _isRecording = true;
                _recordingTestOutputBuilder.Clear();

                await invocation();
                
                stopwatch.Stop();
                if (!MessageBus.QueueMessage(new TestPassed(test, (decimal)stopwatch.Elapsed.TotalSeconds, BuildOutput())))
                {
                    CancellationTokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var duration = (decimal)stopwatch.Elapsed.TotalSeconds;

                if (!MessageBus.QueueMessage(new TestFailed(test, duration, BuildOutput(), ex)))
                {
                    CancellationTokenSource.Cancel();
                }
            }
            finally
            {
                _isRecording = false;
            }

            if (!MessageBus.QueueMessage(new TestFinished(test, (decimal)stopwatch.Elapsed.TotalSeconds, BuildOutput())))
            {
                CancellationTokenSource.Cancel();
            }
        }
    }
}
