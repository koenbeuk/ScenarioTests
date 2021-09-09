using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ScenarioTests
{
    public readonly struct ScenarioTestCaseDescriptor
    {
        public ScenarioTestCaseDescriptor(string name, object? argument, ScenarioTestCaseFlags flags, Func<Task> invocation)
        {
            Name = name;
            Argument = argument;
            Flags = flags;
            Invocation = invocation;
        }

        public string Name { get; }
        public object? Argument { get; }
        public ScenarioTestCaseFlags Flags { get; }
        public Func<Task> Invocation { get; }
    }
}
