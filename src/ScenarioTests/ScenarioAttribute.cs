using System;
using ScenarioTests.Internal;
using Xunit;
using Xunit.Sdk;

namespace ScenarioTests
{
    /// <summary>
    /// Marks this method as a scenario
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [TestCaseOrderer("ScenarioTests.Internal.ScenarioFactTestCaseOrderer", "ScenarioTests")]
    [IgnoreXunitAnalyzersRule1013]
    public sealed class ScenarioAttribute : Attribute
    {
        /// <summary>
        /// Get or set the naming policy for the generated test methods
        /// </summary>
        public ScenarioTestMethodNamingPolicy NamingPolicy { get; set; } = ScenarioTestMethodNamingPolicy.Test;

        /// <summary>
        /// Get or set wether autoabort should be enabled. With AutoAbort, the scenario will abort as soon as possible
        /// </summary>
        public bool AutoAbort { get; set; } = false;

        /// <summary>
        /// Get or set an upper boundary of the max number of theory test cases that can be accepted.
        /// </summary>
        public int TheoryTestCaseLimit { get; set; }
    }
}
