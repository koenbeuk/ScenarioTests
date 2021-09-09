using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioTests
{

    /// <summary>
    /// Configure how the names of tests should be generated
    /// </summary>
    public enum ScenarioTestMethodNamingPolicy
    {
        /// <summary>
        /// TestMethods are generated in the form of [ScenarioName]_[FactName]
        /// </summary>
        MethodAndTest,
        /// <summary>
        /// TestMethods are generated in the form of [FactName]
        /// </summary>
        Test
    }
}
