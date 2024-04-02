using Security.Objects.Entities;
using Security.Data.EF.Interfaces;
using Security.Data.EF;

namespace Security.Data.Brokers.Authentication
{
    public class IdentityBroker : IIdentityBroker
    {
        private readonly ISSODbContextFactory dbContextFactory;

        public IdentityBroker(ISSODbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public SSOUser Me() => 
            ((IdentitySSODbContext)dbContextFactory.CreateDbContext(true))
                .GetCurrentUser();
    }
}
