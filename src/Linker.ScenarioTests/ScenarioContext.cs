using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linker.ScenarioTests.Internal;

namespace Linker.ScenarioTests
{
#pragma warning disable IDE0060 // Remove unused parameter
    public sealed class ScenarioContext
    {
        readonly Func<object[], Func<Task>, Task> _recorder;

        public ScenarioContext(string targetName, Func<object[], Func<Task>, Task> recorder)
        {
            TargetName = targetName;
            _recorder = recorder;
        }

        public string TargetName { get; }

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
                return _recorder(new object[] { }, invocation);
            }

            return Task.CompletedTask;
        }

        [DebuggerStepThrough]
        public void Theory(string name, Action invocation)
            => Theory(name, () =>
            {
                invocation();
                return Task.CompletedTask;
            });

        [DebuggerStepThrough]
        public void Theory<TResult>(string name, Func<TResult> invocation)
            => Theory(name, () =>
            {
                invocation();
                return Task.CompletedTask;
            });

        [DebuggerStepThrough]
        public Task Theory(string name, Func<Task> invocation)
        {
            if (name == TargetName)
            {
                return _recorder(new object[] { }, invocation);
            }

            return Task.CompletedTask;
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
