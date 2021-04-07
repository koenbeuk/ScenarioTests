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

            var scenarioGroups = receiver.ScenarioCandidates
                .Select(x => GetDescriptor(x, context))
                .Where(x => x is not null)
                .GroupBy(x => (x.ClassNamespace, x.ClassName));

            var scenarioSourceBuilder = new StringBuilder();

            foreach (var scenarioGroup in scenarioGroups)
            {
                scenarioSourceBuilder.Clear();

                string RenderScenarios()
                {
                    var scenarioMethodGroups = scenarioGroup
                       .GroupBy(x => (x.MethodName, x.IsAsync));

                    foreach (var scenarioMethodGroup in scenarioMethodGroups)
                    {
                        var invocations = scenarioMethodGroup
                            .SelectMany(x => x.Invocations);

                        foreach (var invocation in invocations)
                        {
                            var invocationName = invocation.FactName ?? $"Fact_{invocation.FactIndex}";

                            if (scenarioMethodGroup.Key.IsAsync)
                            {
                                scenarioSourceBuilder.Append($@"
        [Fact(DisplayName = ""{scenarioMethodGroup.Key.MethodName}_{invocationName}"")]
        [System.Runtime.CompilerServices.CompilerGenerated]
        [System.Diagnostics.DebuggerStepThrough]
        public async ValueTask {scenarioMethodGroup.Key.MethodName}_Fact{invocation.FactIndex}()
        {{
            var scenarioContext = new ScenarioTests.ScenarioContext({invocation.FactIndex}, ""{invocationName}"");
            await {scenarioMethodGroup.Key.MethodName}(scenarioContext).ConfigureAwait(false);
        }}
                        ");
                            }
                            else
                            {

                                scenarioSourceBuilder.Append($@"
        [Fact(DisplayName = ""{scenarioMethodGroup.Key.MethodName}_{invocationName}"")]
        [System.Runtime.CompilerServices.CompilerGenerated]
        [System.Diagnostics.DebuggerStepThrough]
        public void {scenarioMethodGroup.Key.MethodName}_Fact{invocation.FactIndex}()
        {{
            var scenarioContext = new ScenarioTests.ScenarioContext({invocation.FactIndex}, ""{invocationName}"");
            {scenarioMethodGroup.Key.MethodName}(scenarioContext);
        }}
                        ");
                            }
                        }
                    }

                    return scenarioSourceBuilder.ToString();
                }

                var result = 
$@"using System;
using Xunit;

namespace {(string.IsNullOrEmpty(scenarioGroup.Key.ClassNamespace) ? "ScenarioTestsGenerated" : scenarioGroup.Key.ClassNamespace)}
{{
    public partial class {scenarioGroup.Key.ClassName}
    {{        
{RenderScenarios()}      
    }}
}}";
                context.AddSource($"{scenarioGroup.Key.ClassName}.Generated", SourceText.From(result, Encoding.UTF8));
            }
        }

        private static ScenarioDescriptor? GetDescriptor(MethodDeclarationSyntax methodDeclarationSyntax, GeneratorExecutionContext context)
        {
            var semanticModel = context.Compilation.GetSemanticModel(methodDeclarationSyntax.SyntaxTree);
            var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);

            if (methodSymbol is null)
            {
                return null;
            }

            var scenarioAttributeTypeSymbol = context.Compilation.GetTypeByMetadataName("ScenarioTests.ScenarioAttribute");
            var scenarioContextTypeSymbol = context.Compilation.GetTypeByMetadataName("ScenarioTests.ScenarioContext");

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
                            var diagnostic = Diagnostic.Create(Diagnostics.FactNameNeedsToBeAConstant, invocationCandidate.GetLocation(), methodSymbol.Name);
                            context.ReportDiagnostic(diagnostic);
                        }
                        else
                        {
                            factName = factIdExpression.Token.ValueText;
                        }
                    }
                    
                    invocations.Add(new ScenarioInvocationDescriptor
                    {
                        FactName = factName,
                        FactIndex = invocations.Count
                    });
                }
            }

            return new ScenarioDescriptor
            {
                ClassName = methodSymbol.ContainingType.Name,
                ClassNamespace = methodSymbol.ContainingType.ContainingNamespace.Name,
                MethodName = methodSymbol.Name,
                IsAsync = !methodSymbol.ReturnsVoid,
                Invocations = invocations
            };
        }
    }
}
