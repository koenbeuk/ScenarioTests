using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linker.ScenarioTests
{
#pragma warning disable IDE0060 // Remove unused parameter
    public sealed class ScenarioContext
    {
        public ScenarioContext(string targetName = null)
        {
            TargetName = targetName;
        }

        public string? TargetName { get; }

        [DebuggerStepThrough]
        public void Fact(string name, Action invocation)
            => Fact(name, () =>
            {
                invocation();
                return Task.CompletedTask;
            });

        [DebuggerStepThrough]
        public void Fact<TResult>(string name, Func<TResult> invocation)
            => Fact(name, () =>
            {
                invocation();
                return Task.CompletedTask;
            });

        [DebuggerStepThrough]
        public Task Fact(string name, Func<Task> invocation)
        {
            if (name == TargetName)
            {
                return invocation();
            }

            return Task.CompletedTask;
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter

}
