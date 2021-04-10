using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Linker.ScenarioTests.Internal
{
    sealed internal class TheoryTestCaseCollector : IEnumerable<object[]>
    {
        readonly Dictionary<Func<Task>, object[]> _store = new();

        public void Add(Func<Task> factory, object[] testCase)
        {
            _store.Add(factory, testCase);
        }

        public IEnumerator<object[]> GetEnumerator() => _store.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
