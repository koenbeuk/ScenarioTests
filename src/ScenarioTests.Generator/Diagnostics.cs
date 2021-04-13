using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioTests.Generator
{
    public static class Diagnostics
    {
        public static readonly DiagnosticDescriptor RequiresSingleArgumentMethodError = new(
            id: "ST0001",
            title: "Scenario should accept a single argument of type ScenarioContext",
            messageFormat: "Scenario '{0}' should accept a single argument of type ScenarioContext.",
            category: "Design",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor FactOrTheoryNameNeedsToBeAConstant = new(
            id: "ST0002",
            title: "Any fact or theory names need to be expressed as constant literals",
            messageFormat: "Scenario '{3}' has a fact/theory expression that lacks a constant literal name",
            category: "Design",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}
