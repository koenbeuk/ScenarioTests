using Linker.ScenarioTests;
using System;
using Xunit;

namespace TestProject1
{
    public partial class ScenarioTest1
    {
        [Scenario]
        public void Scenario1(ScenarioContext scenario)
        {
            var initialCount = 0;
            var count = initialCount;

            scenario.Fact("ProveInitialState", () =>
            {
                Assert.Equal(initialCount, count);
            });

            count++;
            scenario.Fact("ShouldHaveIncreasedByOne", () =>
            {
                Assert.Equal(1, count);
            });

            scenario.Fact("BeAbleToDecreaseBy1", () =>
            {
                // Manipulate count as part of this fact. this will have no impact on other facts in this scenario
                count--;
                Assert.Equal(initialCount, count);
            });

            count++;
            scenario.Fact("ProveThatWeCanKeepCountingUp", () =>
            {
                // Even through the previous fact decreased our counter, we still remain true to the scenario
                Assert.Equal(2, count);
            });

            for (var i = 0; i < 5; i++)
            {
                scenario.Fact("We can repeat the fact as long as we stay constant", () =>
                {
                    Assert.Equal(2, count);
                });
            }
        }
    }
}
