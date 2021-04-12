using System;
using Linker.ScenarioTests.Internal;
using Xunit;
using Xunit.Sdk;

namespace Linker.ScenarioTests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [IgnoreXunitAnalyzersRule1013]
    public sealed class ScenarioAttribute : Attribute
    {
        public ScenarioTestMethodNamingPolicy NamingPolicy { get; set; } = ScenarioTestMethodNamingPolicy.TypeAndMember;
    }
}
