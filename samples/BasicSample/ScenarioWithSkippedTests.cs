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

            scenario.Fact("We can skip a succeeding test", () =>
            {
                scenario.Skip("Expecting a skip here");

                Assert.True(true);
            });

            scenario.Fact("We can also skip a failing test", () =>
            {
                scenario.Skip("Expecting a skip here");

                Assert.True(true);
            });

            scenario.Skip("We should skip subsequent tests");

            scenario.Fact("This test should always be skipped", () =>
            {
                Assert.True(true);
            });

            return;

#pragma warning disable CS0162 // Unreachable code detected
            scenario.Fact("This test should never be reached and therefore always be skipped", () =>
            {
                Assert.True(true);
            });
#pragma warning restore CS0162 // Unreachable code detected
        }
    }
}
