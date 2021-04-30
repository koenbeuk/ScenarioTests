using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScenarioTests.Internal;

namespace ScenarioTests
{
    /// <summary>
    /// Register facts and theories with your scenario
    /// </summary>
    public sealed class ScenarioContext
    {
        readonly Func<object?, Func<Task>, Task> _recorder;

        string? _skippedReason;

        public ScenarioContext(string targetName, Func<object, Func<Task>, Task> recorder)
        {
            TargetName = targetName;
            _recorder = recorder;
        }

        /// <summary>
        /// Get the name of the current fact or theory that is being executed
        /// </summary>
        public string TargetName { get; }

        /// <summary>
        /// Get when the current test case has been skipped
        /// </summary>
        public bool Skipped => _skippedReason is not null;

        /// <summary>
        /// Get the reason why the current test case was skipped
        /// </summary>
        public string? SkippedReason => _skippedReason;

        /// <summary>
        /// Get if the target test has since been passed 
        /// </summary>
        public bool IsTargetFinished { get; internal set; }

        /// <summary>
        /// Abort the current scenario
        /// </summary>
        public void Abort()
        {
            throw new ScenarioAbortException();
        }

        /// <summary>
        /// Abort the current scenario if we've finished the target test
        /// </summary>
        public void AbortIfTargetIsFinished()
        {
            if (IsTargetFinished)
            {
                Abort();
            }
        }

        [DebuggerStepThrough]
        public void Fact(string name, Action invocation)
            => Fact(name, () =>
            {
                invocation();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="name"></param>
        /// <param name="invocation"></param>
        [DebuggerStepThrough]
        public void Fact<TResult>(string name, Func<TResult> invocation)
            => Fact(name, () =>
            {
                invocation();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

        [DebuggerStepThrough]
        public Task Fact(string name, Func<Task> invocation)
        {
            if (name == TargetName)
            {
                return _recorder(null, invocation);
            }

            return Task.CompletedTask;
        }

        [DebuggerStepThrough]
        public void Theory<T1>(string name, T1 argument, Action invocation)
            => Theory(name, argument, () =>
            {
                invocation();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

        [DebuggerStepThrough]
        public void Theory<T1, TResult>(string name, T1 argument, Func<TResult> invocation)
            => Theory(name, argument, () =>
            {
                invocation();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

        [DebuggerStepThrough]
        public Task Theory<T1>(string name, T1 argument, Func<Task> invocation)
        {
            if (name == TargetName)
            {
                return _recorder(argument, invocation);
            }

            return Task.CompletedTask;
        }

        [DebuggerStepThrough]
        public void Skip(string reason)
        {
            _skippedReason = reason;
        }
    }
}
