using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Linker.ScenarioTests.Internal
{
    sealed internal class ScenarioFactTestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
        readonly IMessageSink _messageSink;

        public ScenarioFactTestCaseDiscoverer(IMessageSink messageSink)
        {


            _messageSink = messageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var sourceInformation = new SourceInformation()
            {
                FileName = factAttribute.GetNamedArgument<string>(nameof(ScenarioFactAttribute.FileName)),
                LineNumber = factAttribute.GetNamedArgument<int>(nameof(ScenarioFactAttribute.LineNumber))
            };

            yield return new ScenarioFactTestCase(_messageSink, discoveryOptions.MethodDisplayOrDefault(), discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, null, sourceInformation);
        }
    }
}
