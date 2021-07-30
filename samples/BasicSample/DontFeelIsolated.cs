using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScenarioTests;
using Xunit;

namespace BasicSample
{
    public partial class DontFeelIsolated
    {
#pragma warning disable CS0618 // Type or member is obsolete
        [Scenario(RunInIsolation = false)]
#pragma warning restore CS0618 // Type or member is obsolete
        public static void PlayScenario(ScenarioContext scenario)
        {
            var counter = 0;

            scenario.Fact("We can start with a default value", () =>
            {
                Assert.Equal(0, counter);
            });

            scenario.Fact("We can Increase to 1", () =>
            {
                counter++;
                Assert.Equal(1, counter);
            });

            scenario.Fact("We can Increase to 2", () =>
            {
                counter++;
                Assert.Equal(2, counter);
            });
        }

        [Scenario]
        public static void PlayScenarioWithSharedFacts(ScenarioContext scenario)
        {
            var counter = 0;

            scenario.SharedFact("We can start with a default value", () =>
            {
                Assert.Equal(0, counter);
            });

            scenario.SharedFact("We can Increase to 1", () =>
            {
                counter++;
                Assert.Equal(1, counter);
            });

            scenario.SharedFact("We can Increase to 2", () =>
            {
                counter++;
                Assert.Equal(2, counter);
            });
        }
    }
}
