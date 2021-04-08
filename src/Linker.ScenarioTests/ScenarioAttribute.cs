using System;
using Linker.ScenarioTests.Internal;
using Xunit;
using Xunit.Sdk;

namespace ScenarioTests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [IgnoreXunitAnalyzersRule1013]
    public sealed class ScenarioAttribute : Attribute
    {
        public ScenarioAttribute()
        {
        }
    }
}
