using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linker.ScenarioTests.Generator
{
    public static class Diagnostics
    {
        public static readonly DiagnosticDescriptor RequiresPartialClassError = new(
            id: "ST0001",
            title: "Requires class to be partial",
            messageFormat: "Class needs to be partial '{0}'.",
            category: "Design",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor RequiresSingleArgumentMethodError = new(
            id: "ST0002",
            title: "Scenario should accept a single argument of type ScenarioContext",
            messageFormat: "Scenario '{0}' should accept a single argument of type ScenarioContext.",
            category: "Design",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor FactNameNeedsToBeAConstant = new(
            id: "ST0003",
            title: "Any facts names need to be expressed as constants",
            messageFormat: "Scenario '{3}' has a fact expression that lacks a constant name",
            category: "Design",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}
