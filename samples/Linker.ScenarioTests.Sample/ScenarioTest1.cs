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
                Assert.Equal(2, count);
            });
        }
    }
}
