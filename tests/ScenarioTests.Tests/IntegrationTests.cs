using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScenarioTests.Internal;
using Xunit;
using Xunit.Runners;

namespace ScenarioTests.Tests
{
    public class IntegrationTests
    {
        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        public void SimpleFact(ScenarioContext scenarioContext)
        {
            scenarioContext.Fact("X", () =>
            {
                Assert.Equal("X", scenarioContext.TargetName);
            });
        }


        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        public void SimpleTheory(ScenarioContext scenarioContext)
        {
            var invocations = 0;

            for (var repeat = 0; repeat < 5; repeat++)
            {
                scenarioContext.Theory("X", repeat, () =>
                {
                    Assert.Equal(0, invocations++);
                });
            }

            Assert.Equal(1, invocations);
        }

        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        public void SimpleTheory2(ScenarioContext scenarioContext)
        {
            var invocations = 0;

            for (var repeat = 0; repeat < 5; repeat++)
            {
                scenarioContext.Theory("X", repeat, () =>
                {
                    Assert.Equal(0, invocations++);
                });
            }

            Assert.Equal(1, invocations);
        }

        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        public async Task DelayedPreconditions(ScenarioContext scenarioContext)
        {
            // We need to manually confirm that indeed the 200ms was added to the total test duration
            await Task.Delay(200);

            scenarioContext.Fact("X", () =>
            {
            });
        }

        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        public async Task DelayedPostconditions(ScenarioContext scenarioContext)
        {
            scenarioContext.Fact("X", () =>
            {
            });

            // We need to manually confirm that indeed the 200ms was added to the total test duration
            await Task.Delay(200);
        }

        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        [Trait("Negate", "true")]
        public void FailingTest(ScenarioContext scenarioContext)
        {
            scenarioContext.Fact("X", () =>
            {
                Assert.False(true);
            });
        }

        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        [Trait("Negate", "true")]
        public async Task FailingAsyncTest(ScenarioContext scenarioContext)
        {
            await scenarioContext.Fact("X", async () =>
            {
                await Task.CompletedTask;
                Assert.False(true);
            });
        }


        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        [Trait("Negate", "true")]
        public void FailingPrecondition(ScenarioContext scenarioContext)
        {
            Assert.True(false);

            scenarioContext.Fact("X", () => { });
        }


        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        [Trait("Negate", "true")]
        public void PreException(ScenarioContext scenarioContext)
        {
            if ((bool)(object)true)
                throw new Exception("Pre");

            scenarioContext.Fact("X", () =>
            {
                Assert.False(true);
            });
        }

        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        [Trait("Negate", "true")]
        public void PostException(ScenarioContext scenarioContext)
        {
            scenarioContext.Fact("X", () => { });

            throw new Exception("Post");
        }


        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        [Trait("Negate", "true")]
        public void FailingTestWithPostException(ScenarioContext scenarioContext)
        {
            scenarioContext.Fact("X", () =>
            {
                Assert.False(true);
            });

            throw new Exception("Post");
        }


        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        [Trait("Negate", "true")]
        public async Task FailingAsyncTestWithPostException(ScenarioContext scenarioContext)
        {
            await scenarioContext.Fact("X", async () =>
            {
                await Task.CompletedTask;
                Assert.False(true);
            });

            throw new Exception("Post");
        }

        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        public void SkippedTest(ScenarioContext scenarioContext)
        {
            scenarioContext.Skip("Foo");

            scenarioContext.Fact("X", () =>
            {
                Assert.False(true);
            });
        }

        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        public void EndScenarioTest(ScenarioContext scenarioContext)
        {
            Assert.Throws<ScenarioAbortException>(() =>
            {
                scenarioContext.EndScenario();

                scenarioContext.Fact("X", () =>
                {
                    Assert.False(true);
                });
            });
        }

        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        public void EndScenarioIfTargetHasFinished(ScenarioContext scenarioContext)
        {
            // Should not throw
            scenarioContext.EndScenarioIfConclusive();

            scenarioContext.Fact("X", () => { });

            Assert.Throws<Internal.ScenarioAbortException>(() =>
            {
                scenarioContext.EndScenarioIfConclusive();
            });
        }

        [Internal.ScenarioFact(FactName = "X", Timeout = 1000)]
        [Trait("Negate", "true")]
        public async Task TimeoutsAreRespectedBeforeOurFact(ScenarioContext scenarioContext)
        {
            await Task.Delay(2000);

            scenarioContext.Fact("X", () => { });
        }

        [Internal.ScenarioFact(FactName = "X", Timeout = 1000)]
        [Trait("Negate", "true")]
        public async Task TimeoutsAreRespectedDuringOurFact(ScenarioContext scenarioContext)
        {
            await scenarioContext.Fact("X", async () => {
                await Task.Delay(2000);
            });
        }

        [Internal.ScenarioFact(FactName = "X", Timeout = 1000, ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        [Trait("Negate", "true")]
        public async Task TimeoutsAreRespectedAfterOurFact(ScenarioContext scenarioContext)
        {
            scenarioContext.Fact("X", () => { });

            await Task.Delay(2000);
        }

        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        [Trait("Negate", "true")]
        public void FailingSharedFactPrecondition(ScenarioContext scenarioContext)
        {
            scenarioContext.SharedFact("Y", () => Assert.False(true));
            scenarioContext.Fact("X", () => { });
        }

        [Internal.ScenarioFact(FactName = "X", ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]
        [Trait("Negate", "true")]
        public void FailingSharedFactPostcondition(ScenarioContext scenarioContext)
        {
            scenarioContext.Fact("X", () => { });
            scenarioContext.SharedFact("Y", () => Assert.False(true));
        }
    }
}
