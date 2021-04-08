using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioTests.Generator
{
    public class ScenarioInvocationDescriptor
    {
        public string FileName { get; set; }
        public int LineNumber { get; set; }
        public string FactName { get; set; }
        public int FactIndex { get; set; }
    }
}
