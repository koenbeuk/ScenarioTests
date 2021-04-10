using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyModel;
using Linker.ScenarioTests;
using Linker.ScenarioTests.Generator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Linker.ScenarioTests.Tests
{
    [UsesVerify]
    public class ScenarioTestGeneratorTests
    {
        readonly ITestOutputHelper _testOutputHelper;

        public ScenarioTestGeneratorTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void EmtpyCode_Noop()
        {
            var compilation = CreateCompilation(@"
class C { }
");

            var result = RunGenerator(compilation);

            Assert.Empty(result.Diagnostics);
            Assert.Empty(result.GeneratedTrees);
        }

        [Fact]
        public void EmptyScenario_AddsScenarioTestHarness()
        {
            var compilation = CreateCompilation(@"
using Linker.ScenarioTests;

partial class C {
    [Scenario]
    public void Scenario(ScenarioContext scenario) {
    }
}
");

            var result = RunGenerator(compilation);

            Assert.Empty(result.Diagnostics);
            Assert.Single(result.GeneratedTrees);
        }

        [Fact]
        public void ScenarioWithIncompatbileArgument_RaisesDiagnosticError()
        {
            var compilation = CreateCompilation(@"
partial class C {
    [Linker.ScenarioTests.Scenario]
    public void Scenario() {
    }
}
");

            var result = RunGenerator(compilation);

            Assert.Contains(result.Diagnostics, x => x.Id == "ST0001");
        }

        [Fact]
        public Task VerifyScenarioWithSingleFact()
        {
            var compilation = CreateCompilation(@"
public partial class C {
    [Linker.ScenarioTests.Scenario]
    public void Scenario(Linker.ScenarioTests.ScenarioContext s) {
        s.Fact(""T1"", () => { });
    }
}
");

            var result = RunGenerator(compilation);

            Assert.Empty(result.Diagnostics);
            Assert.Single(result.GeneratedTrees);

            return Verifier.Verify(result.GeneratedTrees[0].ToString());
        }

        [Fact]
        public Task VerifyScenarioWithMultipleFacts()
        {
            var compilation = CreateCompilation(@"
public partial class C {
    [Linker.ScenarioTests.Scenario]
    public void Scenario(Linker.ScenarioTests.ScenarioContext s) {
        s.Fact(""T1"", () => { });
        s.Fact(""T2"", () => { });
    }
}
");

            var result = RunGenerator(compilation);

            Assert.Empty(result.Diagnostics);
            Assert.Single(result.GeneratedTrees);

            return Verifier.Verify(result.GeneratedTrees[0].ToString());
        }

        [Fact]
        public Task VerifyScenarioWithFactThatReturnsSomething()
        {
            var compilation = CreateCompilation(@"
public partial class C {
    [Linker.ScenarioTests.Scenario]
    public void Scenario(Linker.ScenarioTests.ScenarioContext s) {
        s.Fact(""T1"", () => 1);
    }
}
");

            var result = RunGenerator(compilation);

            Assert.Empty(result.Diagnostics);
            Assert.Single(result.GeneratedTrees);

            return Verifier.Verify(result.GeneratedTrees[0].ToString());
        }

        [Fact]
        public Task VerifyScenarioWithAsyncFact()
        {
            var compilation = CreateCompilation(@"
public partial class C {
    [Linker.ScenarioTests.Scenario]
    public async System.Threading.Tasks.Task Scenario(Linker.ScenarioTests.ScenarioContext s) {
        await s.Fact(""T1"", () => System.Threading.Tasks.Task.CompletedTask);
    }
}
");

            var result = RunGenerator(compilation);

            Assert.Empty(result.Diagnostics);
            Assert.Single(result.GeneratedTrees);

            return Verifier.Verify(result.GeneratedTrees[0].ToString());
        }

        [Fact]
        public Task VerifyScenarioWithMixedFacts()
        {
            var compilation = CreateCompilation(@"
public partial class C {
    [Linker.ScenarioTests.Scenario]
    public async System.Threading.Tasks.Task Scenario(Linker.ScenarioTests.ScenarioContext s) {
        s.Fact(""T1"", () => { });
        s.Fact(""T2"", () => 1);
        await s.Fact(""T3"", () => System.Threading.Tasks.Task.CompletedTask);
    }
}
");

            var result = RunGenerator(compilation);

            Assert.Empty(result.Diagnostics);
            Assert.Single(result.GeneratedTrees);

            return Verifier.Verify(result.GeneratedTrees[0].ToString());
        }

        [Fact]
        public Task VerifyScenarioWithComplexFactName()
        {
            var compilation = CreateCompilation(@"
public partial class C {
    [Linker.ScenarioTests.Scenario]
    public void Scenario(Linker.ScenarioTests.ScenarioContext s) {
        s.Fact(""Name with a $!@$@!"", () => { });
    }
}
");

            var result = RunGenerator(compilation);

            Assert.Empty(result.Diagnostics);
            Assert.Single(result.GeneratedTrees);

            return Verifier.Verify(result.GeneratedTrees[0].ToString());
        }

        [Fact]
        public Task VerifyScenarioWithSingleTheory()
        {
            var compilation = CreateCompilation(@"
public partial class C {
    [Linker.ScenarioTests.Scenario]
    public void Scenario(Linker.ScenarioTests.ScenarioContext s) {
        s.Theory(""T1"", () => { 
            return System.Threading.Tasks.Task.CompletedTask;
        });
    }
}
");

            var result = RunGenerator(compilation);

            Assert.Empty(result.Diagnostics);
            Assert.Single(result.GeneratedTrees);

            return Verifier.Verify(result.GeneratedTrees[0].ToString());
        }

        #region Helpers

        Compilation CreateCompilation(string source, bool expectedToCompile = true)
        {
            var references = Basic.Reference.Assemblies.NetStandard20.All.ToList();
            references.Add(MetadataReference.CreateFromFile(typeof(ScenarioAttribute).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(FactAttribute).Assembly.Location));

            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            var compilation = CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

#if DEBUG

            if (expectedToCompile)
            {
                var compilationDiagnostics = compilation.GetDiagnostics();

                if (!compilationDiagnostics.IsEmpty)
                {
                    _testOutputHelper.WriteLine($"Original compilation diagnostics produced:");

                    foreach (var diagnostic in compilationDiagnostics)
                    {
                        _testOutputHelper.WriteLine($" > " + diagnostic.ToString());
                    }

                    Debug.Fail("Compilation diagnostics produced");
                }
            }
#endif

            return compilation;
        }

        private GeneratorDriverRunResult RunGenerator(Compilation compilation)
        {
            _testOutputHelper.WriteLine("Running generator and updating compilation...");

            var subject = new ScenarioTestGenerator();
            var driver = CSharpGeneratorDriver
                .Create(subject)
                .RunGenerators(compilation);

            var result = driver.GetRunResult();

            if (result.Diagnostics.IsEmpty)
            {
                _testOutputHelper.WriteLine("Run did not produce diagnostics");    
            }
            else
            {
                _testOutputHelper.WriteLine($"Diagnostics produced:");

                foreach (var diagnostic in result.Diagnostics)
                {
                    _testOutputHelper.WriteLine($" > " + diagnostic.ToString());
                }
            }

            foreach (var newSyntaxTree in result.GeneratedTrees)
            {
                _testOutputHelper.WriteLine($"Produced syntax tree with path produced: {newSyntaxTree.FilePath}");
                _testOutputHelper.WriteLine(newSyntaxTree.GetText().ToString());
            }

            return driver.GetRunResult();
        }


        #endregion
    }
}
