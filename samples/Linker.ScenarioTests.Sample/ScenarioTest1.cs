using ScenarioTests;
using System;
using Xunit;

namespace TestProject1
{
    public partial class ScenarioTest1
    {
        [Scenario]
        public void Scenario1(ScenarioContext scenario)
        {
            bool state = false;

            scenario.TestFact(() =>
            {
                Assert.False(state);
            });

            state = true;
            scenario.TestFact(() =>
            {
                Assert.True(state);
                state = false; // Modifying state in this fact does not break the next fact
            });

            scenario.TestFact("EnsureTestsRunInIsolation", () =>
            {
                Assert.True(state);
            });
        }
    }
}
