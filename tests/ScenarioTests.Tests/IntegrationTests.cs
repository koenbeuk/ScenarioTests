using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScenarioTests.Tests
{
    public class IntegrationTests
    {
        [ScenarioFact(DisplayName = nameof(SimpleFact), FactName = "X")]
        public void SimpleFact(ScenarioContext scenarioContext)
        {
            scenarioContext.Fact("X", () =>
            {
                Assert.Equal("X", scenarioContext.TargetName);
            });
        }


        [ScenarioFact(DisplayName = nameof(SimpleTheory), FactName = "X", IsTheory = true)]
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
    }
}
