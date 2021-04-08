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
        public ScenarioContext(int targetIndex, string? targetName = null)
        {
            TargetIndex = targetIndex;
            TargetName = targetName;
        }

        public int CurrentIndex { get; private set; } = -1;

        public int TargetIndex { get; }

        public string? TargetName { get; }

        [DebuggerStepThrough]
        public void Fact(string name, Action invocation)
            => Fact(invocation);

        [DebuggerStepThrough]
        public void Fact(Action invocation)
            => Fact(() =>
            {
                invocation();
                return Task.CompletedTask;
            });

        [DebuggerStepThrough]
        public void Fact<TResult>(string name, Func<TResult> invocation)
            => Fact(invocation);

        [DebuggerStepThrough]
        public void Fact<TResult>(Func<TResult> invocation)
            => Fact(() =>
            {
                invocation();
                return Task.CompletedTask;
            });

        [DebuggerStepThrough]
        public Task Fact(string name, Func<Task> invocation)
            => Fact(invocation);

        [DebuggerStepThrough]
        public Task Fact(Func<Task> invocation)
        {
            CurrentIndex += 1;

            if (CurrentIndex == TargetIndex)
            {
                return invocation();
            }

            return Task.CompletedTask;
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter

}
