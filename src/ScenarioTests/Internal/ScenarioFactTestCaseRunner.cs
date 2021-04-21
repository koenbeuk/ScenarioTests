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
    sealed internal class ScenarioFactTestCaseRunner : XunitTestCaseRunner
    {
        readonly HashSet<object> _testedArguments = new();
        readonly Queue<IMessageSinkMessage> _queuedMessages = new();

        ScenarioContext _scenarioContext;
        bool _skipAdditionalTests;
        bool _pendingRestart;

        void FlushQueuedMessages()
        {
            var outputBuilder = new StringBuilder();

            foreach (var outputMessage in _queuedMessages.OfType<TestOutput>())
            {
                outputBuilder.Append(outputMessage.Output);
            }

            var output = outputBuilder.ToString();

            while (_queuedMessages.Count > 0)
            {
                var message = _queuedMessages.Dequeue();

                var transformedMessage = message switch
                {
                    TestPassed testPassed => new TestPassed(testPassed.Test, testPassed.ExecutionTime, output),
                    TestFailed testFailed => new TestFailed(testFailed.Test, testFailed.ExecutionTime, output, testFailed.ExceptionTypes, testFailed.Messages, testFailed.StackTraces, testFailed.ExceptionParentIndices),
                    TestFinished testFinished => new TestFinished(testFinished.Test, testFinished.ExecutionTime, output),
                    _ => message
                };

                MessageBus.QueueMessage(transformedMessage);
            }
        }

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
        public IMessageSink DiagnosticMessageSink { get; }

        protected override async Task<RunSummary> RunTestAsync()
        {
            var scenarioFactTestCase = (ScenarioFactTestCase)TestCase;
            _scenarioContext = new ScenarioContext(scenarioFactTestCase.FactName, RecordTestCase);

            TestMethodArguments = new object[] { _scenarioContext };

            var filteredMessageBus = new FilteredMessageBus(MessageBus, message =>
            {
                if (message is not ITestStarting and not ITestPassed and not ITestFailed and not ITestFinished )
                {
                    _queuedMessages.Enqueue(message);
                }

                return false;
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

                _queuedMessages.Clear();
                _skipAdditionalTests = false;
                _pendingRestart = false;

                var test = CreateTest(TestCase, DisplayName);
                var result = await CreateTestRunner(test, filteredMessageBus, TestClass, ConstructorArguments, TestMethod, TestMethodArguments, SkipReason, BeforeAfterAttributes, Aggregator, CancellationTokenSource).RunAsync();
                FlushQueuedMessages();

                aggregatedResult.Aggregate(result);
            }
            while (_pendingRestart);

            Console.WriteLine(_pendingRestart);

            return aggregatedResult;
        }

        async Task RecordTestCase(object? argument, Func<Task> invocation)
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

            var testDisplayName = argument is not null ? $"{DisplayName}({argument})" : DisplayName;
            var test = CreateTest(TestCase, testDisplayName);
            var stopwatch = new Stopwatch();

            _queuedMessages.Enqueue(new TestStarting(test));

            stopwatch.Start();

            try
            {
                await invocation();

                stopwatch.Stop();
                _queuedMessages.Enqueue(new TestPassed(test, (decimal)stopwatch.Elapsed.TotalSeconds, null));
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var duration = (decimal)stopwatch.Elapsed.TotalSeconds;

                _queuedMessages.Enqueue(new TestFailed(test, duration, null, ex));
            }
            finally
            {
                _skipAdditionalTests = true;
            }

            _queuedMessages.Enqueue(new TestFinished(test, (decimal)stopwatch.Elapsed.TotalSeconds, null));
        }
    }
}
