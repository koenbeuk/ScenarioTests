using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbIntegrationSample.Models;
using EntityFrameworkCore.Triggered;
using EntityFrameworkCore.Triggered.Extensions;

namespace DbIntegrationSample.Triggers.Users
{
    public class SoftDelete : Trigger<User>
    {
        readonly ApplicationDbContext _applicationDbContext;

        public SoftDelete(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public override void BeforeSave(ITriggerContext<User> context)
        {
            if (context.ChangeType == ChangeType.Deleted)
            {
                context.Entity.DeletedOn = DateTime.UtcNow;
                _applicationDbContext.Entry(context.Entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
        }

    }
}
