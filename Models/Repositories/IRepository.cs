using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Entities;
using Models.Helpers;

namespace Models.Repositories
{
    public interface IRepository<in T>
        where T : class
    {
        Task Migrate(CancellationToken cancellationToken);
        
        
    }
}