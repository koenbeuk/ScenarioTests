using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linker.ScenarioTests.Generator
{
    public class ScenarioInvocationDescriptor
    {
        public string TestMethodName { get; set; }
        public string Name { get; set; }
        public bool IsTheory { get; set; }
        public string FileName { get; set; }
        public int LineNumber { get; set; }
    }
}
