using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ScenarioTests.Internal
{
    public class ScenarioFactTestCaseRunner : XunitTestCaseRunner
    {
        readonly StringBuilder _parentTestOutputBuilder = new StringBuilder();
        readonly StringBuilder _recordingTestOutputBuilder = new StringBuilder();

        ScenarioContext _scenarioContext;
        bool _isRecording;
        bool _skipAdditionalTests;
        bool _pendingRestart;
        HashSet<object> _testedArguments = new();

        public ScenarioFactTestCaseRunner(IXunitTestCase testCase,
                                         string displayName,
                                         string skipReason,
                                         object[] constructorArguments,
                                         IMessageSink diagnosticMessageSink,
                                         IMessageBus messageBus,
                                         ExceptionAggregator aggregator,
                                         CancellationTokenSource cancellationTokenSource)
            : base(testCase, displayName, skipReason, constructorArguments, Array.Empty<object>(), messageBus, aggregator, cancellationTokenSource)
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



            RunSummary aggregatedResult = new();

            _testedArguments.Clear();

            do
            {
                // safeguarding against abuse
                if (_testedArguments.Count > 1000)
                {
                    //throw new ApplicationException("Theory scenario tests are currently capped at 1000 cases (to prevent infinite loops). Feel free to open up an issue if you have a good reason for relaxing this restriction");
                    return aggregatedResult;
                }

                _isRecording = false;
                _skipAdditionalTests = false;
                _pendingRestart = false;
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

                aggregatedResult.Aggregate(result);
            }
            while (_pendingRestart);

            Console.WriteLine(_pendingRestart);

            return aggregatedResult;
        }

        async Task RecordTestCase(object argument, Func<Task> invocation)
        {
            if (_skipAdditionalTests)
            {
                _pendingRestart = true; // when we discovered more tests after a test completed, allow us to restart
                return;
            }

            if (argument is not null)
            {
                if (_testedArguments.Contains(argument))
                {
                    return;
                }

                _testedArguments.Add(argument);
            }

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
                _skipAdditionalTests = true;
            }

            if (!MessageBus.QueueMessage(new TestFinished(test, (decimal)stopwatch.Elapsed.TotalSeconds, BuildOutput())))
            {
                CancellationTokenSource.Cancel();
            }
        }
    }
}
