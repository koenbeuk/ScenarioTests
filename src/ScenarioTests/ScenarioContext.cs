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
        readonly Func<ScenarioTestCaseDescriptor, Task> _recorder;

        string? _skippedReason;

        public ScenarioContext(string targetName, Func<ScenarioTestCaseDescriptor, Task> recorder)
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
        /// Get if the target test has since been passed or skipped
        /// </summary>
        public bool IsTargetConclusive { get; internal set; }

        internal bool AutoAbort { get; set; }

        internal void EndScenarioConditionally()
        {
            if (AutoAbort)
            {
                if (IsTargetConclusive || Skipped)
                {
                    EndScenario();
                }
            }
        }

        /// <summary>
        /// Ends the current scenario
        /// </summary>
        public void EndScenario()
        {
            throw new ScenarioAbortException();
        }

        /// <summary>
        /// Ends the current scenario if we've finished the target test or if we're skipped
        /// </summary>
        public void EndScenarioIfConclusive()
        {
            if (IsTargetConclusive || Skipped)
            {
                EndScenario();
            }
        }

        /// <summary>
        /// Declares a fact to be run by the test runner
        /// </summary>
        /// <param name="name">A unique descriptive name of this test case within the scenario</param>
        /// <param name="invocation">An implementation that futher arrange/assert/acts based on this fact</param>
        [DebuggerStepThrough]
        public void Fact(string name, Action invocation)
            => Fact(name, () =>
            {
                invocation();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

        /// <inheritdoc cref="Fact(string, Action)"/>
        [DebuggerStepThrough]
        public void Fact<TResult>(string name, Func<TResult> invocation)
            => Fact(name, () =>
            {
                invocation();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

        /// <inheritdoc cref="Fact(string, Action)"/>
        [DebuggerStepThrough]
        public async Task Fact(string name, Func<Task> invocation)
        {
            await _recorder(new ScenarioTestCaseDescriptor(name, null, ScenarioTestCaseFlags.Default, invocation));
            EndScenarioConditionally();
        }

        /// <summary>
        /// Declares a shared fact to be run by the test runner
        /// </summary>
        /// <remarks>
        /// Shared facts are similar to facts as that they show up as actual test cases.
        /// In addition, shared facts are also executed for each additional test case which requires them
        /// such that they can be used as preconditions or postconditions.
        /// </remarks>
        /// <param name="name">A unique descriptive name of this test case within the scenario</param>
        /// <param name="invocation">An implementation that futher arrange/assert/acts based on this fact</param>
        public void SharedFact(string name, Action invocation)
            => SharedFact(name, () =>
            {
                invocation();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

        /// <inheritdoc cref="SharedFact(string, Action)"/>
        public TResult SharedFact<TResult>(string name, Func<TResult> invocation)
            => SharedFact(name, () =>
            {
                var result = invocation();
                return Task.FromResult(result);
            }).GetAwaiter().GetResult();

        /// <inheritdoc cref="SharedFact(string, Action)"/>
        public async Task SharedFact(string name, Func<Task> invocation)
            => await SharedFact<object?>(name, async () =>
            {
                await invocation();
                return null;
            });

        /// <inheritdoc cref="SharedFact(string, Action)"/>
        [DebuggerStepThrough]
        public async Task<TResult> SharedFact<TResult>(string name, Func<Task<TResult>> invocation)
        {
            TResult capturedResult = default;

            await _recorder(new ScenarioTestCaseDescriptor(name, null, ScenarioTestCaseFlags.Shared, async () =>
            {
                capturedResult = await invocation();
            }));

            EndScenarioConditionally();

            return capturedResult;
        }

        /// <summary>
        /// Declares a theory be run by the test runner
        /// </summary>
        /// <remarks>
        /// Theories are tests which are fed various bits of data from a data source, 
        /// If the data source contains multiple rows, then the test method is executed multiple times (once with each data row).
        /// Each data row is identified with a uniuqe identifier argument.
        /// </remarks>
        /// <param name="name">A unique descriptive name of this test case within the scenario</param>
        /// <param name="argument">A unique value to identify the current data row</param>
        /// <param name="invocation">An implementation that futher arrange/assert/acts based on this fact</param>
        [DebuggerStepThrough]
        public void Theory<T1>(string name, T1 argument, Action invocation)
            => Theory(name, argument, () =>
            {
                invocation();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

        /// <inheritdoc cref="Theory{T1}(string, T1, Action)"/>
        [DebuggerStepThrough]
        public void Theory<T1, TResult>(string name, T1 argument, Func<TResult> invocation)
            => Theory(name, argument, () =>
            {
                invocation();
                return Task.CompletedTask;
            }).GetAwaiter().GetResult();

        /// <inheritdoc cref="Theory{T1}(string, T1, Action)"/>
        [DebuggerStepThrough]
        public async Task Theory<T1>(string name, T1 argument, Func<Task> invocation)
        {
            await _recorder(new ScenarioTestCaseDescriptor(name, argument, ScenarioTestCaseFlags.Default, invocation));
        }

        /// <summary>
        /// Marks subsequent tests as skipped so that it they will not be run
        /// </summary>
        /// <param name="reason">A reason to be reported</param>
        [DebuggerStepThrough]
        public void Skip(string reason)
        {
            _skippedReason = reason;
            EndScenarioConditionally();
        }
    }
}
