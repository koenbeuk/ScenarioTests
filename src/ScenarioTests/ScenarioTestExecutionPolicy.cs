using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioTests
{
    /// <summary>
    /// Configure when a test in the scenario should end
    /// </summary>
    public enum ScenarioTestExecutionPolicy
    {
        /// <summary>
        /// A scenario test is finished as soon as a satisfyable conclusion has been reached for the current target test
        /// </summary>
        /// <remarks>
        /// Once a test has completed its excecution (regardless of outcome), the Scenario will abort early. 
        /// Any subsequent logic and assertion outside of that test will be discarded with the exception of the Disposal of resources.
        /// This is the likely and wanted behavior as it allows an early test to test a scenario at a certain stage without being affected by subsequent evolutions within that scenario
        /// </remarks>
        EndAfterConclusion,

        /// <summary>
        /// A scenario test is finished when the actual scenario is finished
        /// </summary>
        /// <remarks>
        /// Once a test has completed its execution (regardless of outcome), the Scenario will continue to run until it either Finishes or hits an exception.
        /// Any sybsequent logic and assertion outside that test will still be included in the Test result, with the exception of other Tests.
        /// This behavior is useful if you have some explicit post cleanup/facts that you want to execute for each test.
        /// </remarks>
        EndAfterScenario
    }
}
