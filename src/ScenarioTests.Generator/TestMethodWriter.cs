﻿using System;
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
            StringBuilder.AppendLine($"\t\t{line}");
        }

        public void Write(ScenarioDescriptor scenarioDescriptor, ScenarioInvocationDescriptor scenarioInvocationDescriptor)
        {
            var testMethodName = scenarioDescriptor.NamingPolicy switch
            {
                ScenarioTestMethodNamingPolicy.Test => scenarioInvocationDescriptor.Name,
                _ => $"{scenarioDescriptor.MethodName}_{scenarioInvocationDescriptor.Name}"
            };

            WriteLine("[global::System.Runtime.CompilerServices.CompilerGenerated]");
            WriteLine("[global::System.Diagnostics.DebuggerStepThrough]");
            WriteLine($"[global::ScenarioTests.Internal.ScenarioFact(DisplayName = {Syntax.Literal(testMethodName)}, FactName = {Syntax.Literal(scenarioInvocationDescriptor.Name)}, FileName = {Syntax.Literal(scenarioInvocationDescriptor.FileName)}, LineNumber = {scenarioInvocationDescriptor.LineNumber}, IsTheory = {Syntax.LiteralExpression(scenarioInvocationDescriptor.IsTheory ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)})]");

            if (scenarioDescriptor.IsAsync)
            {
                WriteLine($"public async System.Threading.Tasks.Task {scenarioInvocationDescriptor.TestMethodName}(ScenarioTests.ScenarioContext scenarioContext)");
            }
            else
            {
                WriteLine($"public void {scenarioInvocationDescriptor.TestMethodName}(global::ScenarioTests.ScenarioContext scenarioContext)");
            }
            WriteLine("{");
            
            if (scenarioDescriptor.IsAsync)
            {
                WriteLine($"\tawait {scenarioDescriptor.MethodName}(scenarioContext).ConfigureAwait(false);");
            }
            else
            {
                WriteLine($"\t{scenarioDescriptor.MethodName}(scenarioContext);");
            }

            WriteLine("}");

        }
    }
}