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
            var test = CreateTest(TestCase, DisplayName);
            var aggregatedResult = new RunSummary();

            // Theories are called with required arguments. Keep track of what arguments we already tested so that we can skip those accordingly
            var testedArguments = new HashSet<object>();

            // Each time we find a new theory argument, we will want to restart our Test so that we can collect subsequent test cases
            bool pendingRestart;

            do
            {
                // Safeguarding against abuse
                if (testedArguments.Count >= scenarioFactTestCase.TheoryTestCaseLimit)
                {
                    pendingRestart = false;
                    MessageBus.QueueMessage(new TestSkipped(test, "Theory tests are capped to prevent infinite loops. You can configure a different limit by setting TheoryTestCaseLimit on the Scenario attribute"));
                    aggregatedResult.Aggregate(new RunSummary
                    {
                        Skipped = 1,
                        Total = 1
                    });
                }
                else
                {
                    var bufferedMessageBus = new BufferedMessageBus(MessageBus);
                    var stopwatch = Stopwatch.StartNew();
                    var skipAdditionalTests = false;
                    var testRecorded = false;
                    pendingRestart = false; // By default we dont expect a new restart

                    object? capturedArgument = null;
                    ScenarioContext scenarioContext = null;

                    scenarioContext = new ScenarioContext(scenarioFactTestCase.FactName, async (object? argument, Func<Task> invocation) =>
                    {
                        testRecorded = true;

                        if (skipAdditionalTests)
                        {
                            pendingRestart = true; // when we discovered more tests after a test completed, allow us to restart
                            return;
                        }

                        if (argument is not null)
                        {
                            if (testedArguments.Contains(argument))
                            {
                                return;
                            }

                            testedArguments.Add(argument);
                            capturedArgument = argument;
                        }

                        // At this stage we found our first valid test case, any subsequent test case should issue a restart instead
                        skipAdditionalTests = true;

                        if (scenarioContext.Skipped)
                        {
                            bufferedMessageBus.QueueMessage(new TestSkipped(test, scenarioContext.SkippedReason));
                        }
                        else
                        {
                            try
                            {
                                await invocation();
                            }
                            catch (Exception ex)
                            {
                                bufferedMessageBus.QueueMessage(new TestFailed(test, 0, string.Empty, ex));
                                throw;
                            }
                            finally
                            {
                                if (scenarioContext.Skipped)
                                {
                                    bufferedMessageBus.QueueMessage(new TestSkipped(test, scenarioContext.SkippedReason));
                                }
                            }
                        }
                    });

                    TestMethodArguments = new object[] { scenarioContext };

                    RunSummary result;

                    result = await CreateTestRunner(test, bufferedMessageBus, TestClass, ConstructorArguments, TestMethod, TestMethodArguments, SkipReason, BeforeAfterAttributes, Aggregator, CancellationTokenSource).RunAsync();
                    
                    // We should have expected at least one test run. We probably returned before our target test was able to run
                    if (!testRecorded)
                    {
                        bufferedMessageBus.QueueMessage(new TestSkipped(test, scenarioContext.SkippedReason ?? "No applicable tests were able to run"));
                        result = new RunSummary { Skipped = 1, Total = 1 };
                    }

                    aggregatedResult.Aggregate(result);

                    stopwatch.Stop();
                    var testInvocationTest = capturedArgument switch
                    {
                        null => CreateTest(TestCase, DisplayName),
                        not null => CreateTest(TestCase, $"{DisplayName} ({capturedArgument})")
                    };

                    var bufferedMessages = bufferedMessageBus.QueuedMessages;
                    if (bufferedMessages.OfType<TestSkipped>().Any())
                    {
                        bufferedMessages = bufferedMessages.Where(x => x is not TestPassed and not TestFailed);
                    }

                    if (bufferedMessages.OfType<TestFailed>().Any())
                    {
                        bufferedMessages = bufferedMessages.Where(x => x is not TestPassed);
                    }

                    var output = string.Join("", bufferedMessages
                        .OfType<ITestOutput>()
                        .Select(x => x.Output));

                    var duration = (decimal)stopwatch.Elapsed.TotalSeconds;

                    foreach (var queuedMessage in bufferedMessages)
                    {
                        var transformedMessage = queuedMessage switch
                        {
                            TestStarting testStarting => new TestStarting(testInvocationTest),
                            TestSkipped testSkipped => new TestSkipped(testInvocationTest, testSkipped.Reason),
                            TestPassed testPassed => new TestPassed(testInvocationTest, duration, output),
                            TestFailed testFailed => new TestFailed(testInvocationTest, duration, output, testFailed.ExceptionTypes, testFailed.Messages, testFailed.StackTraces, testFailed.ExceptionParentIndices),
                            TestFinished testFinished => new TestFinished(testInvocationTest, duration, output),
                            _ => queuedMessage
                        };

                        if (!MessageBus.QueueMessage(transformedMessage))
                        {
                            return aggregatedResult;
                        }
                    }
                }
            }
            while (pendingRestart);

            return aggregatedResult;
        }
    }
}
