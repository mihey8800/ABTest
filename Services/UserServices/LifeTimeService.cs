using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Models.Entities;
using Models.Shared;

namespace Services.UserServices
{
    public class CalculateUserLifetimeContext : IServiceContext
    {
        public IEnumerable<User> Users { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }

    public class
        LifetimeService : IService<CalculateUserLifetimeContext, IEnumerable<UserLifetime>>
    {
        public async Task<IEnumerable<UserLifetime>> ExecuteAsync(CalculateUserLifetimeContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserLifetime> Execute(CalculateUserLifetimeContext context)
        {
            return context.Users.Select(s => new UserLifetime
            {
                Lifetime = (s.LastActivityDate - s.RegistrationDate).Days,
                User = s
            }).ToList();
        }
    }
}