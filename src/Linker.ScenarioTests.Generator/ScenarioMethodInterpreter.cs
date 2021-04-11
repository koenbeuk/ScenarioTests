using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linker.ScenarioTests.Generator.TestMethodNamingStrategies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Linker.ScenarioTests.Generator
{
    public sealed class ScenarioMethodInterpreter
    {
        readonly ITestMethodNamingStrategy _testMethodNamingStrategy;

        public ScenarioMethodInterpreter(ITestMethodNamingStrategy testMethodNamingStrategy)
        {
            _testMethodNamingStrategy = testMethodNamingStrategy;
        }

        public ScenarioDescriptor? GetDescriptor(MethodDeclarationSyntax methodDeclarationSyntax, GeneratorExecutionContext context)
        {
            var semanticModel = context.Compilation.GetSemanticModel(methodDeclarationSyntax.SyntaxTree);
            var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);

            if (methodSymbol is null)
            {
                return null;
            }

            var scenarioAttributeTypeSymbol = context.Compilation.GetTypeByMetadataName("Linker.ScenarioTests.ScenarioAttribute");
            var scenarioContextTypeSymbol = context.Compilation.GetTypeByMetadataName("Linker.ScenarioTests.ScenarioContext");
            var asynResultType = context.Compilation.GetSpecialType(SpecialType.System_IAsyncResult);

            var scenarioAttributeClass = methodSymbol.GetAttributes()
                .Where(x => x.AttributeClass.Name == "ScenarioAttribute")
                .FirstOrDefault();

            if (scenarioAttributeClass is null || !SymbolEqualityComparer.Default.Equals(scenarioAttributeClass.AttributeClass, scenarioAttributeTypeSymbol))
            {
                return null;
            }

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
                    string factName = null;

                    if (invocationCandidate.ArgumentList.Arguments.Count > 1)
                    {
                        if (invocationCandidate.ArgumentList.Arguments.FirstOrDefault()?.Expression is not LiteralExpressionSyntax factIdExpression)
                        {
                            var diagnostic = Diagnostic.Create(Diagnostics.FactOrTheoryNameNeedsToBeAConstant, invocationCandidate.GetLocation(), methodSymbol.Name);
                            context.ReportDiagnostic(diagnostic);
                        }
                        else
                        {
                            factName = factIdExpression.Token.ValueText;
                        }
                    }

                    var testMethodName = _testMethodNamingStrategy.GetName(methodSymbol.ContainingType.Name, methodSymbol.Name, factName);
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
                        IsTheory = invocationSymbol.Name == "Theory",
                        FileName = methodDeclarationSyntax.SyntaxTree.FilePath,
                        LineNumber = invocationCandidate.GetLocation().GetMappedLineSpan().StartLinePosition.Line + 1
                    });
                }
            }

            return new ScenarioDescriptor
            {
                ClassName = methodSymbol.ContainingType.Name,
                ClassNamespace = methodSymbol.ContainingType.ContainingNamespace.IsGlobalNamespace ? null : methodSymbol.ContainingType.ContainingNamespace.ToDisplayString(),
                MethodName = methodSymbol.Name,
                IsAsync = methodSymbol.ReturnsVoid ? false : methodSymbol.ReturnType.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, asynResultType)),
                Invocations = invocations
            };
        }
    }
}
