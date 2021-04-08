using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Linker.ScenarioTests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("Linker.ScenarioTests.Internal.ScenarioFactTestCaseDiscoverer", "Linker.ScenarioTests")]
    public sealed class ScenarioFactAttribute : FactAttribute
    {
        public string FileName { get; set; }
        public int LineNumber { get; set; }
    }
}
