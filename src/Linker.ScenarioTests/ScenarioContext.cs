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
        public void TestFact(string name, Action factInvocation)
            => TestFact(factInvocation);

        [DebuggerStepThrough]
        public void TestFact(Action factInvocation)
            => TestFact(() =>
            {
                factInvocation();
                return Task.CompletedTask;
            });

        [DebuggerStepThrough]
        public void TestFact<TResult>(string name, Func<TResult> factInvocation)
            => TestFact(factInvocation);

        [DebuggerStepThrough]
        public void TestFact<TResult>(Func<TResult> factInvocation)
            => TestFact(() =>
            {
                factInvocation();
                return Task.CompletedTask;
            });

        [DebuggerStepThrough]
        public Task TestFact(string name, Func<Task> factInvocation)
            => TestFact(factInvocation);

        [DebuggerStepThrough]
        public Task TestFact(Func<Task> factInvocation)
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
