﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ScenarioTests.Generator
{
    public static class ScenarioMethodInterpreter
    {
        public static ScenarioDescriptor? GetDescriptor(MethodDeclarationSyntax methodDeclarationSyntax, GeneratorExecutionContext context)
        {
            var semanticModel = context.Compilation.GetSemanticModel(methodDeclarationSyntax.SyntaxTree);
            var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);

            if (methodSymbol is null)
            {
                return null;
            }

            var scenarioAttributeTypeSymbol = context.Compilation.GetTypeByMetadataName("ScenarioTests.ScenarioAttribute");
            var scenarioContextTypeSymbol = context.Compilation.GetTypeByMetadataName("ScenarioTests.ScenarioContext");
            var asynResultType = context.Compilation.GetSpecialType(SpecialType.System_IAsyncResult);

            var scenarioAttributeClass = methodSymbol.GetAttributes()
                .Where(x => x.AttributeClass.Name == "ScenarioAttribute")
                .FirstOrDefault();

            if (scenarioAttributeClass is null || !SymbolEqualityComparer.Default.Equals(scenarioAttributeClass.AttributeClass, scenarioAttributeTypeSymbol))
            {
                return null;
            }

            var namingPolicy = scenarioAttributeClass.NamedArguments
                .Where(x => x.Key == "NamingPolicy")
                .Where(x => x.Value.Kind == TypedConstantKind.Enum)
                .Select(x => x.Value.Value)
                .Where(x => Enum.IsDefined(typeof(ScenarioTestMethodNamingPolicy), x))
                .Cast<ScenarioTestMethodNamingPolicy>()
                .FirstOrDefault();

            var executionPolicy = scenarioAttributeClass.NamedArguments
                .Where(x => x.Key == "ExecutionPolicy")
                .Where(x => x.Value.Kind == TypedConstantKind.Enum)
                .Select(x => x.Value.Value)
                .Where(x => Enum.IsDefined(typeof(ScenarioTestExecutionPolicy), x))
                .Cast<ScenarioTestExecutionPolicy>()
                .FirstOrDefault();

            var runInIsolation = scenarioAttributeClass.NamedArguments
                .Where(x => x.Key == "RunInIsolation")
                .Where(x => x.Value.Kind == TypedConstantKind.Primitive)
                .Where(x => x.Value.Value is bool)
                .Select(x => (bool)x.Value.Value)
                .DefaultIfEmpty(true)
                .First();

            var theoryTestCaseLimit = scenarioAttributeClass.NamedArguments
                .Where(x => x.Key == "TheoryTestCaseLimit")
                .Where(x => x.Value.Kind == TypedConstantKind.Primitive)
                .Select(x => x.Value.Value)
                .OfType<int?>()
                .FirstOrDefault();

            var timeout = scenarioAttributeClass.NamedArguments
                .Where(x => x.Key == "Timeout")
                .Where(x => x.Value.Kind == TypedConstantKind.Primitive)
                .Select(x => x.Value.Value)
                .OfType<int?>()
                .FirstOrDefault();

            if (methodSymbol.Parameters.Length != 1 || !SymbolEqualityComparer.Default.Equals(methodSymbol.Parameters[0].Type, scenarioContextTypeSymbol))
            {
                var diagnostic = Diagnostic.Create(Diagnostics.RequiresSingleArgumentMethodError, methodDeclarationSyntax.GetLocation(), methodSymbol.Name);
                context.ReportDiagnostic(diagnostic);
                return null;
            }

            var invocations = new List<ScenarioInvocationDescriptor>();

            foreach (var invocationCandidate in methodDeclarationSyntax.Body.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                var invocationSymbol = semanticModel.GetSymbolInfo(invocationCandidate).Symbol as IMethodSymbol;
                if (invocationSymbol is not null && SymbolEqualityComparer.Default.Equals(invocationSymbol.ContainingType, scenarioContextTypeSymbol))
                {
                    if (invocationSymbol.Name is not "Fact" and not "SharedFact" and not "Theory")
                    {
                        // We only want to generate test cases for facts and theories
                        continue;
                    }

                    string factName = null;

                    if (invocationCandidate.ArgumentList.Arguments.Count > 1)
                    {
                        if (invocationCandidate.ArgumentList.Arguments.FirstOrDefault()?.Expression is not LiteralExpressionSyntax factIdExpression)
                        {
                            var diagnostic = Diagnostic.Create(Diagnostics.FactOrTheoryNameNeedsToBeAConstant, invocationCandidate.GetLocation(), methodSymbol.Name);
                            context.ReportDiagnostic(diagnostic);
                            continue;
                        }
                     
                        factName = factIdExpression.Token.ValueText;
                    }

                    if (factName is not null)
                    {
                        if (invocations.Any(x => x.Name == factName))
                        {
                            var diagnostic = Diagnostic.Create(Diagnostics.FactOrTheoryNameNeedsToBeUnique, invocationCandidate.GetLocation(), methodSymbol.Name);
                            context.ReportDiagnostic(diagnostic);
                            continue;
                        }

                        var testMethodName = TestMethodNamingStrategy.GetName(methodSymbol.ContainingType.Name, methodSymbol.Name, factName);
                        var originalTestMethodName = testMethodName;
                        var queryDuplicated = invocations.Where(x => x.TestMethodName.Equals(testMethodName, StringComparison.Ordinal));
                        var duplicatedCounter = 0;

                        while (queryDuplicated.Any())
                        {
                            duplicatedCounter += 1;
                            testMethodName = $"originalTestMethodName{duplicatedCounter}";
                        }

                        invocations.Add(new ScenarioInvocationDescriptor
                        {
                            TestMethodName = testMethodName,
                            Name = factName,
                            FileName = methodDeclarationSyntax.SyntaxTree.FilePath,
                            LineNumber = invocationCandidate.GetLocation().GetMappedLineSpan().StartLinePosition.Line + 1
                        });
                    }
                }
            }

            return new ScenarioDescriptor
            {
                NamingPolicy = namingPolicy,
                ClassName = methodSymbol.ContainingType.Name,
                ClassNamespace = methodSymbol.ContainingType.ContainingNamespace.IsGlobalNamespace ? null : methodSymbol.ContainingType.ContainingNamespace.ToDisplayString(),
                MethodName = methodSymbol.Name,
                IsAsync = !methodSymbol.ReturnsVoid && methodSymbol.ReturnType.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, asynResultType)),
                TheoryTestCaseLimit = theoryTestCaseLimit,
                ExecutionPolicy = executionPolicy,
                RunInIsolation = runInIsolation,
                Timeout = timeout,
                Invocations = invocations
            };
        }
    }
}
