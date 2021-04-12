using Linker.ScenarioTests;
using Xunit;

namespace AwesomeCalculator
{
    partial class ScenarioTests
    {
        [Scenario(NamingPolicy = ScenarioTestMethodNamingPolicy.Test)]
        public void BasicUsage(ScenarioContext scenario)
        {
            var calculator = new Calculator();

            scenario.Fact("Calculator starts in an initial state of 0", () =>
            {
                Assert.Equal(0, calculator.State);
            });

            calculator.Add(1);
            scenario.Fact("We can add a number", () =>
            {
                Assert.Equal(1, calculator.State);
            });

            calculator.Undo();
            scenario.Fact("We can undo our last action if we want", () =>
            {
                // Facts run in isolution, any change here will not affect other facts
                Assert.Equal(0, calculator.State);
            });

            var specialNumbers = new[] { decimal.Zero, decimal.MinusOne, decimal.One, decimal.MinValue, decimal.MaxValue };
            foreach (var specialNumber in specialNumbers)
            {
                scenario.Theory("We can add a special number without issues", specialNumber, () =>
                {
                    calculator.Add(specialNumber);
                    Assert.Equal(calculator.State, specialNumber);
                });
            }
        }
    }
}



