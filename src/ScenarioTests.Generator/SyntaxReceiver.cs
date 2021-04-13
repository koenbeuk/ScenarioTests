using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace ScenarioTests.Generator
{
    public class SyntaxReceiver : ISyntaxReceiver
    {
        public List<MethodDeclarationSyntax> ScenarioCandidates { get; } = new List<MethodDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // any method with a scenario attribute is a candidate for test generation
            if (syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax && methodDeclarationSyntax.AttributeLists.Count > 0)
            {
                var hasScenarioAttribute = methodDeclarationSyntax.AttributeLists
                    .SelectMany(x => x.Attributes)
                    .Any(x => x.Name.ToString().Contains("Scenario"));

                if (hasScenarioAttribute)
                {
                    ScenarioCandidates.Add(methodDeclarationSyntax);
                }
            }
        }
    }
}
