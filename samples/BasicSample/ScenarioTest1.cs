using ScenarioTests;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace BasicSample
{
    public partial class ScenarioTest1
    {
        readonly ITestOutputHelper _testOutputHelper;

        public ScenarioTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Scenario(NamingPolicy = ScenarioTestMethodNamingPolicy.Test)]
        public void Scenario1(ScenarioContext scenario)
        {
            _testOutputHelper.WriteLine("Setting initial count to 0");

            var initialCount = 0;
            var count = initialCount;

            scenario.Fact("ProveInitialState", () =>
            {
                _testOutputHelper.WriteLine("Proving that initial count is still 0");
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
                scenario.Theory("TheorySample", 1, () =>
                {
                    var newCount = count + i;

                    Assert.Equal(count, newCount - i);
                });
            }
        }
    }
}
