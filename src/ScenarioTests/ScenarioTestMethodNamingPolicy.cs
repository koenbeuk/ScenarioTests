using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioTests
{
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
