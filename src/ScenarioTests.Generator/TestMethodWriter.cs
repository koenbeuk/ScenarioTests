using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Syntax = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ScenarioTests.Generator
{
    public class TestMethodWriter
    {
        public TestMethodWriter(StringBuilder stringBuilder)
        {
            StringBuilder = stringBuilder;
        }

        public StringBuilder StringBuilder { get; }

        void WriteLine(string line)
        {
            StringBuilder.Append("\t\t");
            StringBuilder.AppendLine(line);
        }

        public void Write(ScenarioDescriptor scenarioDescriptor, ScenarioInvocationDescriptor scenarioInvocationDescriptor)
        {
            var testMethodName = scenarioDescriptor.NamingPolicy switch
            {
                ScenarioTestMethodNamingPolicy.Test => scenarioInvocationDescriptor.Name,
                _ => $"{scenarioDescriptor.MethodName}_{scenarioInvocationDescriptor.Name}"
            };

            var theoryAttributes = scenarioDescriptor.TheoryTestCaseLimit is not null ? $", TheoryTestCaseLimit = {scenarioDescriptor.TheoryTestCaseLimit}" : string.Empty;

            WriteLine("[global::System.Runtime.CompilerServices.CompilerGenerated]");
            WriteLine("[global::System.Diagnostics.DebuggerStepThrough]");
            WriteLine($"[global::ScenarioTests.Internal.ScenarioFact(DisplayName = {Syntax.Literal(testMethodName)}, FactName = {Syntax.Literal(scenarioInvocationDescriptor.Name)}, ExecutionPolicy = global::ScenarioTests.ScenarioTestExecutionPolicy.{scenarioDescriptor.ExecutionPolicy}, RunInIsolation = {(scenarioDescriptor.RunInIsolation ? "true" : "false")}, FileName = {Syntax.Literal(scenarioInvocationDescriptor.FileName)}, LineNumber = {scenarioInvocationDescriptor.LineNumber}{theoryAttributes})]");

            if (scenarioDescriptor.IsAsync)
            {
                WriteLine($"public async System.Threading.Tasks.Task {scenarioInvocationDescriptor.TestMethodName}(global::ScenarioTests.ScenarioContext scenarioContext)");
            }
            else
            {
                WriteLine($"public void {scenarioInvocationDescriptor.TestMethodName}(global::ScenarioTests.ScenarioContext scenarioContext)");
            }
            WriteLine("{");
            WriteLine("\ttry");
            WriteLine("\t{");

            if (scenarioDescriptor.IsAsync)
            {
                WriteLine($"\t\tawait {scenarioDescriptor.MethodName}(scenarioContext).ConfigureAwait(false);");
            }
            else
            {
                WriteLine($"\t\t{scenarioDescriptor.MethodName}(scenarioContext);");
            }

            WriteLine("\t}");
            WriteLine("\tcatch(global::ScenarioTests.Internal.ScenarioAbortException) { }");
            WriteLine("}");

        }
    }
}
