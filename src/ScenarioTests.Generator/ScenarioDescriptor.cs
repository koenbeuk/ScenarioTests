using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioTests.Generator
{
    public class ScenarioDescriptor
    {
        public ScenarioTestMethodNamingPolicy NamingPolicy { get; set; }

        public string ClassNamespace { get; set; }
        public string ClassName { get; set; }

        public string MethodName { get; set; }
        public bool IsAsync { get; set; }
        public int? TheoryTestCaseLimit { get; set; }
        public ScenarioTestExecutionPolicy ExecutionPolicy { get; set; }
        public bool RunInIsolation { get; set; } = true;
        public int? Timeout { get; set; }    

        public List<ScenarioInvocationDescriptor> Invocations { get; set; }
    }
}
