using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Models.Entities;
using Models.Shared;

namespace Services.UserServices
{
    public class CalculateRollingRetentionContext : IServiceContext
    {
        public int DaysCount { get; set; }
        public IEnumerable<User> Users { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }

    public class
        CalculateRollingRetentionService : IService<CalculateRollingRetentionContext, IEnumerable<UserRetention>>
    {
        public async Task<IEnumerable<UserRetention>> ExecuteAsync(CalculateRollingRetentionContext context)
        {
            var now = DateTime.Now;
            var startingDay = now.AddDays(-context.DaysCount);
            var datesList = Enumerable.Range(0, 1 + now.Subtract(startingDay).Days)
                .Select(offset => startingDay.AddDays(offset))
                .ToList();

            var retentions = new BlockingCollection<UserRetention>();
            var tasks = datesList.Select(x => new Task(() =>
            {
                var liveUsers = context.Users.Count(user => user.RegistrationDate <= x && user.LastActivityDate >= x);
                retentions.Add(new UserRetention()
                {
                    LiveUsers = liveUsers,
                    Day = x
                }, context.CancellationToken);
            })).ToArray();
            await Task.WhenAll(tasks);
            return retentions;
        }

        public IEnumerable<UserRetention> Execute(CalculateRollingRetentionContext context)
        {
            var now = DateTime.Now;
            var startingDay = now.AddDays(-context.DaysCount);
            var datesList = Enumerable.Range(0, 1 + now.Subtract(startingDay).Days)
                .Select(offset => startingDay.AddDays(offset))
                .ToList();

            var retentions = new BlockingCollection<UserRetention>();
            Parallel.ForEach(datesList, (x) =>
            {
                var liveUsers = context.Users.Count(user => user.RegistrationDate <= x && user.LastActivityDate >= x);
                retentions.Add(new UserRetention()
                {
                    LiveUsers = liveUsers,
                    Day = x
                }, context.CancellationToken);
            });
            //return retentions;

            var userRetentions = new List<UserRetention>();

            while (startingDay < now)
            {
                var liveUsers = context.Users.Count(user =>
                    user.RegistrationDate <= startingDay && user.LastActivityDate >= startingDay);

                userRetentions.Add(new UserRetention()
                {
                    LiveUsers = liveUsers,
                    Day = startingDay
                });
                startingDay = startingDay.AddDays(1);
            }


            return userRetentions;
        }
    }
}