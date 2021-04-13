using System;
using System.Linq;
using System.Threading.Tasks;
using ScenarioTests.Internal;
using Xunit;

namespace ScenarioTests.Tests
{
    public class ScenarioContextTests
    {
        [Fact]
        public void Fact_ScenarioWithMultipleFacts_InvokesWhenHit()
        {
            var context = new ScenarioContext("X2", (_, i) => i());

            context.Fact("X1", () => throw new Exception());

            var invoked = false;
            context.Fact("X2", () =>
            {
                invoked = true;
            });

            context.Fact("X3", () => throw new Exception());

            Assert.True(invoked);
        }

        [Fact]
        public void Fact_NoReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", (_, i) => i());

            var invoked = false;
            context.Fact("X", () =>
            {
                invoked = true;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void Fact_WithReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", (_, i) => i());
             
            var invoked = false;
            context.Fact("X", () =>
            {
                invoked = true;
                return 1;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void Fact_WithAsyncNoReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", (_, i) => i());

            var invoked = false;
            context.Fact("X", () =>
            {
                invoked = true;
                return Task.CompletedTask;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void Fact_WithAsyncAndReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", (_, i) => i());

            var invoked = false;
            context.Fact("X", () =>
            {
                invoked = true;
                return Task.FromResult(1);
            });

            Assert.True(invoked);
        }

        [Fact]
        public void Fact_WithNameNoReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", (_, i) => i());

            var invoked = false;
            context.Fact("X", () =>
            {
                invoked = true;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void Fact_WithNameWithReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", (_, i) => i());

            var invoked = false;
            context.Fact("X", () =>
            {
                invoked = true;
                return 1;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void Fact_WithNameWithAsyncNoReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", (_, i) => i());

            var invoked = false;
            context.Fact("X", () =>
            {
                invoked = true;
                return Task.CompletedTask;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void Fact_WithNameWithAsyncAndReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", (_, i) => i());

            var invoked = false;
            context.Fact("X", () =>
            {
                invoked = true;
                return Task.FromResult(1);
            });

            Assert.True(invoked);
        }

        [Fact]
        public void Theory_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", (_, i) => i());

            var invoked = false;
            context.Theory("X", 1, () =>
            {
                invoked = true;
                return Task.FromResult(1);
            });

            Assert.True(invoked);
        }
    }
}
