using System;
using System.Linq;
using DbIntegrationSample.Models;
using ScenarioTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DbIntegrationSample
{
    public partial class ScenarioTests
    {
        [Scenario(NamingPolicy = ScenarioTestMethodNamingPolicy.Test)]
        public void ICanAddAndRemoveUsers(ScenarioContext scenario)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(scenario.TargetName);
                    options.UseTriggers(triggerOptions =>
                    {
                        triggerOptions.AddTrigger<Triggers.Users.SoftDelete>(); // This will ensure that users are getting soft deleted
                    });
                })
                .BuildServiceProvider();

            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            scenario.Fact("Initially there are no users", () =>
            {
                Assert.Equal(0, dbContext.Users.Count());
            });

            var user = new User { UserName = "Scott" };
            dbContext.Add(user);
            dbContext.SaveChanges();

            scenario.Fact("There is now 1 user", () =>
            {
                Assert.Equal(1, dbContext.Users.Count());
            });

            // Our trigger will rewrite this delete into a soft-delete
            dbContext.Remove(user);
            dbContext.SaveChanges();

            scenario.Fact("We still have our single user", () =>
            {
                Assert.Equal(1, dbContext.Users.Count());
            });

            scenario.Fact("User has been assigned a deleted date", () =>
            {
                dbContext.Entry(user).Reload(); // Make sure that we get the latest from the database
                Assert.NotNull(user.DeletedOn);
            });
        }
    }
}
