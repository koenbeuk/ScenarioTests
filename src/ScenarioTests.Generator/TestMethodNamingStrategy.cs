using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioTests.Generator
{
    public static class TestMethodNamingStrategy
    {
        static string Sanitize(string literal)
        {
            return string.Join("", literal
                .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '_')
                .Select(c => char.IsLetterOrDigit(c) ? c : '_')
            );
        }

        public static string GetName(string className, string scenarioName, string invocationName)
        {
            return Sanitize($"{scenarioName}_{invocationName}");
        }
    }
}
