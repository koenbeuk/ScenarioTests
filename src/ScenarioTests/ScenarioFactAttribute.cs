using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace ScenarioTests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("ScenarioTests.Internal.ScenarioFactTestCaseDiscoverer", "ScenarioTests")]
    [TestCaseOrderer("ScenarioTests.Internal.ScenarioFactTestCaseOrderer", "ScenarioTests")]
    public sealed class ScenarioFactAttribute : FactAttribute
    {
        public string FactName { get; set; }
        public string FileName { get; set; }
        public int LineNumber { get; set; }
        public bool IsTheory { get; set; }
    }
}
