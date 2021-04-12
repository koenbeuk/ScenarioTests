# Linker.ScenarioTests
Triggers for EF Core. Respond to changes in your DbContext before and after they are committed to the database.

[![NuGet version (Linker.ScenarioTests)](https://img.shields.io/nuget/v/Linker.ScenarioTests.XUnit.svg?style=flat-square)](https://www.nuget.org/packages/Linker.ScenarioTests.XUnit/)
[![Build status](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/workflows/.NET%20Core/badge.svg)](https://github.com/linkerio/Linker.ScenarioTests/actions?query=workflow%3A%22.NET+Core%22)

## NuGet packages
- Linker.ScenarioTests.XUnit [![NuGet version](https://img.shields.io/nuget/v/Linker.ScenarioTests.XUnit.svg?style=flat-square)](https://www.nuget.org/packages/Linker.ScenarioTests.XUnit/) [![NuGet](https://img.shields.io/nuget/dt/Linker.ScenarioTests.XUnit.svg?style=flat-square)](https://www.nuget.org/packages/Linker.ScenarioTests.XUnit/)

## Getting started
1. Run `dotnet new xunit`
2. Install the package from [NuGet](https://www.nuget.org/packages/Linker.ScenarioTests.XUnit)
3. Make your test class
3. Register your triggers with your DbContext
4. View our [samples](https://github.com/linkerio/Linker.ScenarioTests/blob/master/samples/AwesomeCalculator/Program.cs)

### Example
```csharp
partial class ScenarioTests
{
    [Scenario]
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
            // Facts run in isolution, any change here will not affect other facts and theories
            Assert.Equal(0, calculator.State);
        });

        scenario.Fact("This fact is broken...", () => {
            // This fact will fail however it will not affect other facts and theories
            Assert.Equal(1, calculator.State);
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

        // More steps are available in the sample....
    }
}
```

### What you get
![Screenshot of experience in VS2019](/assets/images/sample1.png "Experience in VS2019")

### How it works
We have a source generator that checks for methods in your test class marked with the `[Scenario]` attribute. When it finds one, it ensures that it has a single argument that accepts a `ScenarioContext`. 

It will then keep on discovering calls in the shape of `ScenarioContext.Fact` or `ScenarioContext.Theory` and generate individual test cases for those. If you add `<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>` in your csproj, you can see the code that gets generated. Each generated test case is harnassed to now affect other test cases.

Theory test cases are internally isolated. A theory in the shape of:

```csharp
for (var index = 0; index < 3; i++) {
    scenario.Theory("Must be even", index, () => {
        Assert.True(index % 2 == 0);
    });
}
```

will generate 3 test cases: 0, 1, and 2 of which 1 will fail. Theories require an identifier for each individual test case, in this example the identifier is the value of `index`. An identifier can be anything that is constant within an app domain. This can include a number, string, database identifier or even a tuple composing multiple values.


### TODO

Export more in this readme soon