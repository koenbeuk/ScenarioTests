using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linker.ScenarioTests.Internal;
using Xunit;

namespace Linker.ScenarioTests.Tests.Internal
{
    public class TheoryTestCaseCollectorTests
    {
        //[Fact]
        //public async Task AddsAreSynchronizedWithReads()
        //{
        //    using var subject = new TheoryTestCaseCollector();
        //    var written = false;

        //    var readingTask = Task.Run(() =>
        //    {
        //        _ = subject.FirstOrDefault();
        //        Assert.True(written);
        //    });

        //    var writingTask = Task.Run(() =>
        //    {
        //        written = true;
        //        subject.Add(new object[] { 1 });
        //    });

        //    await Task.WhenAll(readingTask, writingTask); 
        //}
    }
}
