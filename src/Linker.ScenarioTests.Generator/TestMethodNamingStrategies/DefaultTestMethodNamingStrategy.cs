using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linker.ScenarioTests.Generator.TestMethodNamingStrategies
{
    public sealed class DefaultTestMethodNamingStrategy : ITestMethodNamingStrategy
    {
        string Sanitize(string literal)
        {
            return string.Join("", literal
                .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '_')
                .Select(c => char.IsLetterOrDigit(c) ? c : '_')
            );
        }

        public string GetName(string className, string scenarioName, string invocationName)
        {
            return Sanitize($"{scenarioName}_{invocationName}");
        }
    }
}
