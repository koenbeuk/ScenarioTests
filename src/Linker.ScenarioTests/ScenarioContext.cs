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
        public void Fact(string name, Action factInvocation)
            => Fact(factInvocation);

        [DebuggerStepThrough]
        public void Fact(Action factInvocation)
            => Fact(() =>
            {
                factInvocation();
                return Task.CompletedTask;
            });

        [DebuggerStepThrough]
        public void Fact<TResult>(string name, Func<TResult> factInvocation)
            => Fact(factInvocation);

        [DebuggerStepThrough]
        public void Fact<TResult>(Func<TResult> factInvocation)
            => Fact(() =>
            {
                factInvocation();
                return Task.CompletedTask;
            });

        [DebuggerStepThrough]
        public Task Fact(string name, Func<Task> factInvocation)
            => Fact(factInvocation);

        [DebuggerStepThrough]
        public Task Fact(Func<Task> factInvocation)
        {
            CurrentIndex += 1;

            if (CurrentIndex == TargetIndex)
            {
                return factInvocation();
            }

            return Task.CompletedTask;
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter

}
