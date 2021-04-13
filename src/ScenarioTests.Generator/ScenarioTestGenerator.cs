using ScenarioTests.Generator;
using ScenarioTests.Generator.TestMethodNamingStrategies;
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

namespace ScenarioTests.Generator
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

            var resultBuilder = new StringBuilder();
            foreach (var scenarioGroup in scenarioGroups)
            {
                resultBuilder.Clear();
                if (!string.IsNullOrEmpty(scenarioGroup.Key.ClassNamespace))
                {
                    resultBuilder.Append("namespace ");
                    resultBuilder.AppendLine(scenarioGroup.Key.ClassNamespace);
                    resultBuilder.AppendLine("{");
                }

                resultBuilder.AppendLine($@"
    [global::Xunit.TestCaseOrderer(""ScenarioTests.Internal.ScenarioFactTestCaseOrderer"", ""ScenarioTests"")]
    public partial class {scenarioGroup.Key.ClassName}
    {{");

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

                resultBuilder.Append(@"
    }");

                if (!string.IsNullOrEmpty(scenarioGroup.Key.ClassNamespace))
                {
                    resultBuilder.Append(@"
}");
                }

                context.AddSource($"{scenarioGroup.Key.ClassName}.Generated", SourceText.From(resultBuilder.ToString(), Encoding.UTF8));
            }
        }
    }
}
