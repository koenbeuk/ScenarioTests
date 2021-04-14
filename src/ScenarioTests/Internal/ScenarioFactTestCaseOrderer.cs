using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ScenarioTests.Internal
{
    public class ScenarioFactTestCaseOrderer : ITestCaseOrderer
    {

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            return testCases
                .Select(testCase => (
                    testCase,
                    testCaseAttribute: testCase.TestMethod
                        .Method
                        .GetCustomAttributes(typeof(ScenarioFactAttribute))
                        .FirstOrDefault()
                ))
                .OrderBy(x => x.testCase.TestMethod.TestClass.Class.Name)
                .ThenBy(x => x.testCase.TestMethod.Method.Name)
                .ThenBy(x => x.testCaseAttribute?.GetNamedArgument<string>(nameof(ScenarioFactAttribute.FileName)))
                .OrderBy(x => x.testCaseAttribute?.GetNamedArgument<int>(nameof(ScenarioFactAttribute.LineNumber)))
                .Select(x => x.testCase);
        }
    }
}
