using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Syntax = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Linker.ScenarioTests.Generator.TestMethodWriters
{
    public class TestMethodWriter
    {
        public TestMethodWriter(StringBuilder stringBuilder, ScenarioDescriptor scenarioDescriptor, ScenarioInvocationDescriptor scenarioInvocationDescriptor)
        {
            StringBuilder = stringBuilder;
            ScenarioDescriptor = scenarioDescriptor;
            ScenarioInvocationDescriptor = scenarioInvocationDescriptor;
        }

        public StringBuilder StringBuilder { get; }
        public ScenarioDescriptor ScenarioDescriptor { get; }
        public ScenarioInvocationDescriptor ScenarioInvocationDescriptor { get; }

        void WriteLine(string line)
        {
            StringBuilder.AppendLine($"\t\t{line}");
        }

        public void Write()
        {
            WriteLine("[System.Runtime.CompilerServices.CompilerGenerated]");
            WriteLine("[System.Diagnostics.DebuggerStepThrough]");
            WriteLine($"[Linker.ScenarioTests.ScenarioFact(DisplayName = {Syntax.Literal($"{ScenarioDescriptor.MethodName}_{ScenarioInvocationDescriptor.Name}")}, FileName = {Syntax.Literal(ScenarioInvocationDescriptor.FileName)}, LineNumber = {ScenarioInvocationDescriptor.LineNumber})]");
            WriteLine($"public async System.Threading.Tasks.Task {ScenarioInvocationDescriptor.TestMethodName}()");
            WriteLine("{");
            WriteLine($"\tvar scenarioContext = new Linker.ScenarioTests.ScenarioContext({Syntax.Literal(ScenarioInvocationDescriptor.Name)});");

            if (ScenarioDescriptor.IsAsync)
            {
                WriteLine($"\tawait {ScenarioDescriptor.MethodName}(scenarioContext).ConfigureAwait(false);");
            }
            else
            {
                WriteLine($"\t{ScenarioDescriptor.MethodName}(scenarioContext);");
                WriteLine($"\tawait System.Threading.Tasks.Task.CompletedTask;"); // make sure that we use an await statement in here to prevent compiler warnings
            }

            WriteLine("}");

        }
    }
}
