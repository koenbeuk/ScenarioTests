using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ScenarioTests.Internal
{
    class ScenarioFactTestCaseOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            return testCases
                .OrderBy(x => x.TestMethod.TestClass.Class.Name)
                .ThenBy(x => x.TestMethod.Method.Name)
                .ThenBy(x => x.SourceInformation.FileName)
                .ThenBy(x => x.SourceInformation.LineNumber);
        }
    }
}
