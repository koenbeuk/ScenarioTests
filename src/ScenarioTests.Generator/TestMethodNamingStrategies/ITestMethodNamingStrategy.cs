using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioTests.Generator.TestMethodNamingStrategies
{
    public interface ITestMethodNamingStrategy
    {
        string GetName(string className, string scenarioName, string invocationName);
    }
}
