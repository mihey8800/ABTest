using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Models.Entities;
using Models.Helpers;

namespace Models.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<PagedList<User>> GetUserList(CancellationToken cancellationToken, UserParameters userParameters);
        Task<IEnumerable<User>> GetUserList(CancellationToken cancellationToken);
        Task<User> GetUser(int id, CancellationToken cancellationToken);
        Task<User> CreateUser(User item, CancellationToken cancellationToken);
        Task CreateUsers(IEnumerable<User> items, CancellationToken cancellationToken);
        Task<User> UpdateUser(User item, CancellationToken cancellationToken);
        Task UpdateUsers(IEnumerable<User> items, CancellationToken cancellationToken);
        Task<bool> DeleteUser(int id, CancellationToken cancellationToken);
    }
}