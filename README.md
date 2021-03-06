# ScenarioTests
ScenarioTests are a different way of writing tests with XUnit. The goal is to be able to write tests like you would write notebooks. ScenarioTests are great for documentation and integration/e2e tests.

[![NuGet version (ScenarioTests)](https://img.shields.io/nuget/v/ScenarioTests.XUnit.svg?style=flat-square)](https://www.nuget.org/packages/ScenarioTests.XUnit/)
[![Build status](https://github.com/koenbeuk/ScenarioTests/actions/workflows/build.yml/badge.svg)](https://github.com/koenbeuk/ScenarioTests/actions/workflows/build.yml)

## NuGet packages
- ScenarioTests.XUnit [![NuGet version](https://img.shields.io/nuget/v/ScenarioTests.XUnit.svg?style=flat-square)](https://www.nuget.org/packages/ScenarioTests.XUnit/) [![NuGet](https://img.shields.io/nuget/dt/ScenarioTests.XUnit.svg?style=flat-square)](https://www.nuget.org/packages/ScenarioTests.XUnit/)

## Getting started
1. Create a new XUnit test project
2. Install the nuget package from [NuGet](https://www.nuget.org/packages/ScenarioTests.XUnit)
3. Create a new test class and implement a scenario
5. View our [samples](https://github.com/koenbeuk/ScenarioTests/blob/master/samples/) and read the [introduction post](https://onthedrift.com/posts/scenario-tests/)

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
            calculator.Add(5);
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

The generator will then keep on discovering calls in the shape of `ScenarioContext.Fact` or `ScenarioContext.Theory` and generate individual test methods for those calls. Each generated test case is harnassed to not affect other test cases as it will Invoke the ScenarioMethod with an ScenarioContext instance that is configured to Ignore any tests not named after the Test for which this TestMethod is generated.

If you add `<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>` in your csproj, you can see the code that gets generated.

There are 2 execution policies available (Configured on the Scenario attribute, e.g. `[Scenario(ExecutionPolicy = ScenarioTestExecutionPolicy.EndAfterScenario)]`):
- `EndAfterScenario` will run the Scenario for a particular test case and it will not complete until the entire scenario has been played out.
- `EndAfterConclusion` (Default) will run the Scenario for a particular test case up until a satisfactory conclusion has been reached. This can happen either if our test case has been verified or when our test case has been Skipped. This works by raining a `ScenarioAbortException` right after we've concluded an outcome for a particular test case. This exception is then silently ignored. 

Theory test cases are internally isolated. A theory in the shape of:

```csharp
for (var index = 0; index < 3; i++) {
    scenario.Theory("Must be even", index, () => {
        Assert.True(index % 2 == 0);
    });
}
```

will generate 3 test cases: 0, 1, and 2 of which 1 will fail. Theories require an identifier for each individual test case, in this example the identifier is the value of `index`. An identifier can be anything that is constant within an app domain. This can include a number, string, database identifier or even a tuple composing multiple values.

### FAQ

#### How can I log additional output
As this is an extension on XUnit, you can you add a constructor accepting an `ITestOutputHelper` which is an XUnit primitive for writing additional output. As an example:
```csharp
class TestHost {
    readonly ITestOutputHelper _out;

    public TestHost(ITestOutputHelper testOutputHelper) { _out = testOutputHelper; }    

    public void Scenario1(ScenarioContext scenario) {
        _out.WriteLine("Something that gets written for all tests...");

        scenario.Fact("Fact1", () => {
            _out.WriteLine("This only gets written for Fact1");
        });

        scenario.Theory("Theory1", 1, () => {
            _out.WriteLine("This is only written for a test case for this theory with identity 1");
        });
    }
}
```

#### Can I return data from my tests
Yes and no, its perfectly valid for a fact or theory to return something but it will get ignored. You can do a return from within a Fact or theory but you cant capture its value. You can manipulate the state of outside components from within a fact or theory however this will not affect other tests.

#### Can I have preconditions and postconditions that are validated for all tests
Yes; you can Assert both within and outside of tests. Consider this example:
```csharp
public void Scenario1(ScenarioContext scenario) {
    // Prefix running for each test case
    var database = CreateTestDatabase();
    Assert.True(database.IsCreated);
    
    try {
        scenario.Fact("Ensure that we start with 0 users", () => {
            Assert.Equal(0, database.Users.Count());
        });

        // Add a single users, our subsequent facts will need it...
        database.Users.Add(new User("Scott"));

        scenario.Fact("Ensure that we added a user", () => {
            Assert.Equal(1, database.Users.Count());
        });
    }
    finally {
        // Postfix running for each test case
        database.Destoy();
        Assert.True(database.IsDestroyed);
    }
}
```

Tests will fail if the database fails to be created or destroyed. Preconditions running before the target test will always be evaluated. Post conditions will only be evaluated if they are in a catch/finally block or if the ExecutionPolicy of your scenario is set to `EndAfterScenario`.

#### Can I have async facts
Certainly, there are overloads for facts and theories that return a task; an example:
```csharp
public async Task Scenario1(ScenarioContext scenario) {
    var database = await CreateTestDatabase();

    await scenario.Fact("Ensure that we start with 0 users", async () => {
        Assert.Equal(0, await database.Users.CountAsync());
    });
}
```

#### Is this compatible with MSTest/NUnit/....
Currently we only expose a generator for XUnit. We'd like to produce generators for different testing frameworks in the future however we have no direct need for this. If this is important for you then please go ahead and open an issue or take a swing at it yourself!

#### Can I skip tests?
You can call `scenario.Skip("reason...")` before- during or after a test.
```csharp
public async Task Scenario1(ScenarioContext scenario) {
    var database = CreateTestDatabase();
    
    if (!database.Created) {
        scenario.Skip("Was not able to create a test database...");
    }
    else {
        await scenario.Fact("Ensure that we start with 0 users", async () => {
            if (....) {
                scenario.Skip("We're skipping this test because of... reasons");
            }

            Assert.Equal(0, await database.Users.CountAsync());
        });
    }
}
```
