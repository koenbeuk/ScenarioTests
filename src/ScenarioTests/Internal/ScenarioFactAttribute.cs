using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ScenarioTests.Internal
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("ScenarioTests.Internal.ScenarioFactTestCaseDiscoverer", "ScenarioTests")]
    [TestCaseOrderer("ScenarioTests.Internal.ScenarioFactTestCaseOrderer", "ScenarioTests")]
    public sealed class ScenarioFactAttribute : FactAttribute
    {
        public string FactName { get; set; }
        public ScenarioTestExecutionPolicy ExecutionPolicy { get; set; }
        public bool RunInIsolation { get; set; }
        public int TheoryTestCaseLimit { get; set; } = 100;
        public string FileName { get; set; }
        public int LineNumber { get; set; }
    }
}
