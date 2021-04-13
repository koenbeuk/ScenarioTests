using System;
using ScenarioTests.Internal;
using Xunit;
using Xunit.Sdk;

namespace ScenarioTests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [TestCaseOrderer("ScenarioTests.Internal.ScenarioFactTestCaseOrderer", "ScenarioTests")]
    [IgnoreXunitAnalyzersRule1013]
    public sealed class ScenarioAttribute : Attribute
    {
        public ScenarioTestMethodNamingPolicy NamingPolicy { get; set; } = ScenarioTestMethodNamingPolicy.MethodAndTest;
    }
}
