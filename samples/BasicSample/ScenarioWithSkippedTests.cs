using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScenarioTests;
using Xunit;

namespace BasicSample
{
    public partial class ScenarioWithSkippedTests
    {
        [Scenario]
        public void Scenario1(ScenarioContext scenario)
        {
            scenario.Fact("We can test before our requirements", () =>
            {
                Assert.True(true);
            });

            scenario.Fact("We can skip a particular test", () =>
            {
                scenario.Skip("Expecting a skip here");

                Assert.True(true);
            });

            scenario.Skip("We should skip subsequent tests");

            scenario.Fact("This test should always be skipped", () =>
            {
                Assert.True(true);
            });

        }
    }
}
