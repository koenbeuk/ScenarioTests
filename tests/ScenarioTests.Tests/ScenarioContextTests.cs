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
            var context = new ScenarioContext("X", d => d.Invocation());

            System.Threading.Thread.Sleep(5000);

            _ = context.Fact("X1", () => throw new Exception());

            var invoked = false;
            context.Fact("X2", () =>
            {
                invoked = true;
            });


            _ = context.Fact("X3", () => throw new Exception());

            Assert.True(invoked);
        }

        [Fact]
        public void Fact_NoReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", d => d.Invocation());

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
            var context = new ScenarioContext("X", d => d.Invocation());

            var invoked = false;
            context.Fact("X", () =>
            {
                invoked = true;
                return 1;
            });

            Assert.True(invoked);
        }

        [Fact]
        public async Task Fact_WithAsyncNoReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", d => d.Invocation());

            var invoked = false;
            await context.Fact("X", () =>
            {
                invoked = true;
                return Task.CompletedTask;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void Fact_WithAsyncAndReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", d => d.Invocation());

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
            var context = new ScenarioContext("X", d => d.Invocation());

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
            var context = new ScenarioContext("X", d => d.Invocation());

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
            var context = new ScenarioContext("X", d => d.Invocation());

            var invoked = false;
            _ = context.Fact("X", () =>
            {
                invoked = true;
                return Task.CompletedTask;
            });

            Assert.True(invoked);
        }

        [Fact]
        public void Fact_WithNameWithAsyncAndReturnType_InvokesWhenHit()
        {
            var context = new ScenarioContext("X", d => d.Invocation());

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
            var context = new ScenarioContext("X", d => d.Invocation());

            var invoked = false;
            context.Theory("X", 1, () =>
            {
                invoked = true;
                return Task.FromResult(1);
            });

            Assert.True(invoked);
        }

        [Fact]
        public void Theory_MultipleDataRows_InvokesAll()
        {
            var context = new ScenarioContext("X", d => d.Invocation());

            var invoked = 0;
            for (var i = 0; i < 5; i++)
            {
                context.Theory("X", i, () =>
                {
                    invoked++;
                });
            }

            Assert.Equal(5, invoked);
        }

        [Fact]
        public void SharedFact_InvokesWhenTargeted()
        {
            var context = new ScenarioContext("X", d => d.Invocation());
            var invoked = false;
            
            context.SharedFact("X", () => invoked = true);

            Assert.True(invoked);
        }

        [Fact]
        public void SharedFact_InvokesAsAPrecondition_WhenNotTargeted()
        {
            var context = new ScenarioContext("X", d => d.Invocation());
            var invoked = false;

            context.SharedFact("Y", () => invoked = true);
            // A dummy target
            context.Fact("X", () => { });

            Assert.True(invoked);
        }

        [Fact]
        public void SharedFact_InvokesAsAPostcondition_WhenNotTargeted()
        {
            var context = new ScenarioContext("X", d => d.Invocation());
            var invoked = false;

            // A dummy target
            context.Fact("X", () => { });
            context.SharedFact("Y", () => invoked = true);

            Assert.True(invoked);
        }

        [Fact]
        public void SharedFact_SkippedWhenNotInLineAndNotTargeted()
        {
            var context = new ScenarioContext("X", d => d.Invocation());
            var invoked = false;

            // The shared fact is never reached
            if ((bool)(object)false)
            {
                context.SharedFact("Y", () => invoked = true);
            }

            // A dummy target
            context.Fact("X", () => { });

            Assert.False(invoked);
        }

        [Fact]
        public void SharedFact_CanReturnValues()
        {
            var context = new ScenarioContext("X", d => d.Invocation());

            var result = context.SharedFact("X", () => 1);

            Assert.Equal(1, result);
        }


        [Fact]
        public async Task SharedFact_Async_CanReturnValues()
        {
            var context = new ScenarioContext("X", d => d.Invocation());

            var result = await context.SharedFact("X", () => Task.FromResult(1));

            Assert.Equal(1, result);
        }
    }
}
