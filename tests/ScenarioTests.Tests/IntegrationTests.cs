using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Runners;

namespace ScenarioTests.Tests
{
    public class IntegrationTests
    {
        [Internal.ScenarioFact(FactName = "X")]
        public void SimpleFact(ScenarioContext scenarioContext)
        {
            scenarioContext.Fact("X", () =>
            {
                Assert.Equal("X", scenarioContext.TargetName);
            });
        }


        [Internal.ScenarioFact(FactName = "X")]
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

        [Internal.ScenarioFact(FactName = "X")]
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

        [Internal.ScenarioFact(FactName = "X")]
        public async Task DelayedPreconditions(ScenarioContext scenarioContext)
        {
            // We need to manually confirm that indeed the 200ms was added to the total test duration
            await Task.Delay(200);

            scenarioContext.Fact("X", () =>
            {
            });
        }

        [Internal.ScenarioFact(FactName = "X")]
        public async Task DelayedPostconditions(ScenarioContext scenarioContext)
        {
            scenarioContext.Fact("X", () =>
            {
            });

            // We need to manually confirm that indeed the 200ms was added to the total test duration
            await Task.Delay(200);
        }

        [Internal.ScenarioFact(FactName = "X")]
        [Trait("Negate", "true")]
        public void FailingTest(ScenarioContext scenarioContext)
        {
            scenarioContext.Fact("X", () =>
            {
                Assert.False(true);
            });
        }

        [Internal.ScenarioFact(FactName = "X")]
        [Trait("Negate", "true")]
        public async Task FailingAsyncTest(ScenarioContext scenarioContext)
        {
            await scenarioContext.Fact("X", async () =>
            {
                await Task.CompletedTask;
                Assert.False(true);
            });
        }

        [Internal.ScenarioFact(FactName = "X")]
        [Trait("Negate", "true")]
        public void FailingTestWithPostException(ScenarioContext scenarioContext)
        {
            scenarioContext.Fact("X", () =>
            {
                Assert.False(true);
            });

            throw new Exception();
        }

        [Internal.ScenarioFact(FactName = "X")]
        public void SkippedTest(ScenarioContext scenarioContext)
        {
            scenarioContext.Skip("Foo");

            scenarioContext.Fact("X", () =>
            {
            });
        }
    }
}
