using Linker.ScenarioTests.Generator;
using Linker.ScenarioTests.Generator.TestMethodNamingStrategies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syntax = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Linker.ScenarioTests.Generator
{
    [Generator]
    public class ScenarioTestGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) =>
            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver 
            if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            {
                return;
            }

            var scenarioMethodInterpreter = new ScenarioMethodInterpreter(new DefaultTestMethodNamingStrategy());

            var scenarioGroups = receiver.ScenarioCandidates
                .Select(x => scenarioMethodInterpreter.GetDescriptor(x, context))
                .Where(x => x is not null)
                .GroupBy(x => (x.ClassNamespace, x.ClassName));

            foreach (var scenarioGroup in scenarioGroups)
            {
                string RenderScenarios()
                {
                    var resultBuilder = new StringBuilder();
                    var testMethodWriter = new TestMethodWriter(resultBuilder);

                    var scenarioMethodGroups = scenarioGroup
                       .GroupBy(x => x.MethodName);

                    foreach (var scenarioMethodGroup in scenarioMethodGroups)
                    {
                        var scenario = scenarioMethodGroup.First();
                        var invocations = scenarioMethodGroup
                            .SelectMany(x => x.Invocations);

                        foreach (var invocation in invocations)
                        {
                            testMethodWriter.Write(scenario, invocation);
                        }
                    }

                    return resultBuilder.ToString();
                }
                 
                var result = 
$@"using System;
using Xunit;

namespace {(string.IsNullOrEmpty(scenarioGroup.Key.ClassNamespace) ? "ScenarioTestsGenerated" : scenarioGroup.Key.ClassNamespace)}
{{
    [TestCaseOrderer(""Linker.ScenarioTests.Internal.ScenarioFactTestCaseOrderer"", ""Linker.ScenarioTests"")]
    public partial class {scenarioGroup.Key.ClassName}
    {{        
{RenderScenarios()}      
    }}
}}";
                context.AddSource($"{scenarioGroup.Key.ClassName}.Generated", SourceText.From(result, Encoding.UTF8));
            }
        }
    }
}
