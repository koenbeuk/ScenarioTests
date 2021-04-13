using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScenarioTests.Internal;

namespace ScenarioTests
{
#pragma warning disable IDE0060 // Remove unused parameter
    public sealed class ScenarioContext
    {
        readonly Func<object, Func<Task>, Task> _recorder;

        public ScenarioContext(string targetName, Func<object, Func<Task>, Task> recorder)
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
                return _recorder(Array.Empty<object>(), invocation);
            }

            return Task.CompletedTask;
        }

        [DebuggerStepThrough]
        public void Theory<T1>(string name, T1 argument, Action invocation)
            => Theory(name, argument, () =>
            {
                invocation();
                return Task.CompletedTask;
            });

        [DebuggerStepThrough]
        public void Theory<T1, TResult>(string name, T1 argument, Func<TResult> invocation)
            => Theory(name, argument, () =>
            {
                invocation();
                return Task.CompletedTask;
            });

        [DebuggerStepThrough]
        public Task Theory<T1>(string name, T1 argument, Func<Task> invocation)
        {
            if (name == TargetName)
            {
                return _recorder(argument, invocation);
            }

            return Task.CompletedTask;
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
