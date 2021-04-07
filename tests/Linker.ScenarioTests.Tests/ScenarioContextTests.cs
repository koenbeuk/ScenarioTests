using System;
using System.Threading.Tasks;
using Xunit;

namespace ScenarioTests.Tests
{
    public class ScenarioContextTests
    {
        [Fact]
        public void TestFact_MultipleFacts_InvokesWhenHit()
        {
            var context = new ScenarioContext(1);

            context.TestFact(() => throw new Exception());

            var invoked = false;
            context.TestFact(() =>
            {
                invoked = true;
            });

            context.TestFact(() => throw new Exception());

            Assert.True(invoked);
        }

        [Fact]
        public void TestFact_NoReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext(0);

            var invoked = false;
            context.TestFact(() =>
            {
                invoked = true;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void TestFact_WithReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext(0);

            var invoked = false;
            context.TestFact(() =>
            {
                invoked = true;
                return 1;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void TestFact_WithAsyncNoReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext(0);

            var invoked = false;
            context.TestFact(() =>
            {
                invoked = true;
                return Task.CompletedTask;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void TestFact_WithAsyncAndReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext(0);

            var invoked = false;
            context.TestFact(() =>
            {
                invoked = true;
                return Task.FromResult(1);
            });

            Assert.True(invoked);
        }

        [Fact]
        public void TestFact_WithNameNoReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext(0);

            var invoked = false;
            context.TestFact("X", () =>
            {
                invoked = true;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void TestFact_WithNameWithReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext(0);

            var invoked = false;
            context.TestFact("X", () =>
            {
                invoked = true;
                return 1;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void TestFact_WithNameWithAsyncNoReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext(0);

            var invoked = false;
            context.TestFact("X", () =>
            {
                invoked = true;
                return Task.CompletedTask;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void TestFact_WithNameWithAsyncAndReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext(0);

            var invoked = false;
            context.TestFact("X", () =>
            {
                invoked = true;
                return Task.FromResult(1);
            });

            Assert.True(invoked);
        }
    }
}
