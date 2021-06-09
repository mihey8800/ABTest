using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Contexts;
using Models.Entities;
using Models.Helpers;

namespace Models.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<PostgresSqlContext> _dbFactory;

        public UserRepository(IDbContextFactory<PostgresSqlContext> contextFactory)
        {
            _dbFactory = contextFactory;
        }

        public async Task Migrate(CancellationToken cancellationToken)
        {
            await using var db = _dbFactory.CreateDbContext();
            await db.Database.MigrateAsync(cancellationToken);
        }


        public async Task<PagedList<User>> GetUserList(CancellationToken cancellationToken, UserParameters userParameters)
        {
            await using var db = _dbFactory.CreateDbContext();
            return PagedList<User>.ToPagedList(db.Users.OrderBy(o => o.Id), userParameters.PageNumber, userParameters.PageSize);
        }

        public async Task<IEnumerable<User>> GetUserList(CancellationToken cancellationToken)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.Users.ToListAsync(cancellationToken);
        }

        public async Task<User> GetUser(int id, CancellationToken cancellationToken)
        {
            await using var db = _dbFactory.CreateDbContext();
            return await db.Users.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<User> CreateUser(User item, CancellationToken cancellationToken)
        {
            try
            {
                await using var db = _dbFactory.CreateDbContext();
                var user = await db.Users.AddAsync(item, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
                return user.Entity;
            }
            catch
            {
                //here will be good to add logging
                return null;
            }
        }

        public async Task CreateUsers(IEnumerable<User> items, CancellationToken cancellationToken)
        {
            try
            {
                await using var db = _dbFactory.CreateDbContext();
                await db.Users.AddRangeAsync(items, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                //here will be good to add logging
            }
        }


        public async Task<User> UpdateUser(User item, CancellationToken cancellationToken)
        {
            try
            {
                await using var db = _dbFactory.CreateDbContext();
                var user = db.Users.Update(item);
                await db.SaveChangesAsync(cancellationToken);
                return user.Entity;
            }
            catch
            {
                //here will be good to add logging
                return null;
            }
        }

        public async Task UpdateUsers(IEnumerable<User> items, CancellationToken cancellationToken)
        {
            try
            {
                await using var db = _dbFactory.CreateDbContext();


                db.Users.UpdateRange(items);

                await db.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                //here will be good to add logging
            }
        }


        public async Task<bool> DeleteUser(int id, CancellationToken cancellationToken)
        {
            try
            {
                await using var db = _dbFactory.CreateDbContext();
                var user = await db.Users.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
                db.Users.Remove(user);
                var saved = await db.SaveChangesAsync(cancellationToken);
                return saved > 0;
            }
            catch
            {
                //here will be good to add logging
                return false;
            }
        }
    }
}