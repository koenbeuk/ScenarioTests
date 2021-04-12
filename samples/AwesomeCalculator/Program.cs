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

            var specialNumbers = new[] { 0d, -1d, 1d, double.MinValue, double.MaxValue, double.PositiveInfinity, double.NegativeInfinity };
            foreach (var specialNumber in specialNumbers)
            {
                scenario.Theory("We can add a special number without issues", specialNumber, () =>
                {
                    // As each test runs in isolution, we do not need to undo or reset the calculator after our manipulation
                    calculator.Add(specialNumber);
                    Assert.Equal(calculator.State, specialNumber);
                });
            }

            calculator.Divide(0);

            scenario.Fact("Division by 0 leaves the calculated in a NaN state", () =>
            {
                Assert.True(double.IsNaN(calculator.State));
            });

            scenario.Fact("Calculator is now in an error state", () =>
            {
                Assert.True(calculator.HasError);
            });

            scenario.Fact("Undoing the division will bring the calculator out of the error state", () =>
            {
                calculator.Undo();
                Assert.False(calculator.HasError);
            });

            calculator.Reset();
            scenario.Fact("Reset also breaks it out of the error state", () =>
            {
                calculator.Reset();
                Assert.False(calculator.HasError);
            });
        }
    }
}



