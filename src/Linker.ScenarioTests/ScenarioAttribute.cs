using System;
using Xunit;
using Xunit.Sdk;

namespace ScenarioTests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("ScenarioTests.Internal.ScenarioTestDiscoverer", "ScenarioTests")]
    public sealed class ScenarioAttribute : FactAttribute
    {
     
    }
}
