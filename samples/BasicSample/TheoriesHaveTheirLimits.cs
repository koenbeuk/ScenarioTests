using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScenarioTests;
using Xunit;


namespace BasicSample
{
    public partial class TheoriesHaveTheirLimits : IClassFixture<TheoriesHaveTheirLimits.ClassFixture>
    {

        public class ClassFixture
        {
            public int CurrentRepeatCount { get; set; }
        }

        readonly ClassFixture _classFixture;

        public TheoriesHaveTheirLimits(ClassFixture classFixture)
        {
            _classFixture = classFixture;
        }

        [Scenario(TheoryTestCaseLimit = 5)]
        public void Scenario(ScenarioContext scenario)
        {
            var currentRepeatCount = 0;

            // 500 being an arbritrary limit that exceeds the configured 5 and default 100
            for (var i = 0; i < 500; i++)
            {
                scenario.Theory("We can repeat this theory for all data entries", i, () =>
                {
                    Assert.Equal(0, currentRepeatCount);
                    Assert.Equal(i, _classFixture.CurrentRepeatCount);

                    currentRepeatCount += 1;
                    _classFixture.CurrentRepeatCount += 1;
                });
            }

            Assert.True(_classFixture.CurrentRepeatCount <= 5);
        }
    }
}
