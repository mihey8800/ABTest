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
    public class CalculateRollingRetentionXdayContext : IServiceContext
    {
        public int DaysCount { get; set; }
        public IEnumerable<User> Users { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }

    public class
        RollingRetentionXdayService : IService<CalculateRollingRetentionXdayContext, IEnumerable<RollingRetentionXDay>>
    {
        public async Task<IEnumerable<RollingRetentionXDay>> ExecuteAsync(CalculateRollingRetentionXdayContext context)
        {
            var now = DateTime.Now;
            var startingDay = now.AddDays(-context.DaysCount);
            var datesList = Enumerable.Range(0, now.Subtract(startingDay).Days)
                .Select(offset => startingDay.AddDays(offset))
                .ToList();


            var retentions = new BlockingCollection<RollingRetentionXDay>();
            var tasks = datesList.Select(x =>
                    Task.Run(() => AddToRetentionsList(retentions, context, x, context.CancellationToken)))
                .ToArray();
            await Task.WhenAll(tasks);
            return retentions.OrderBy(o => o.Day).ToList();
        }


        public void AddToRetentionsList(BlockingCollection<RollingRetentionXDay> retentions,
            CalculateRollingRetentionXdayContext context, DateTime day, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var installsCount = context.Users.Count(user => user.RegistrationDate <= day);
            var returnsCount = context.Users.Count(user => user.LastActivityDate >= day);

            retentions.Add(new RollingRetentionXDay()
            {
                Percent = installsCount > 0 ? (returnsCount / (double) installsCount) : 0,
                Day = day
            }, context.CancellationToken);
        }

        public IEnumerable<RollingRetentionXDay> Execute(CalculateRollingRetentionXdayContext context)
        {
            var now = DateTime.Now;
            var startingDay = now.AddDays(-context.DaysCount);
            var datesList = Enumerable.Range(0, now.Subtract(startingDay).Days)
                .Select(offset => startingDay.AddDays(offset))
                .ToList();

            var retentions = new BlockingCollection<RollingRetentionXDay>();
            Parallel.ForEach(datesList, (x) =>
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                var installsCount = context.Users.Count(user => user.RegistrationDate <= x);
                var returnsCount = context.Users.Count(user => user.LastActivityDate >= x);

                retentions.Add(new RollingRetentionXDay()
                {
                    Percent = installsCount > 0 ? (returnsCount / (double) installsCount) : 0,
                    Day = x
                }, context.CancellationToken);
            });
            return retentions.OrderBy(o => o.Day).ToList();
        }
    }
}