using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IService<in TContext>
        where TContext : IServiceContext
    {
        Task ExecuteAsync(TContext context);
        void Execute(TContext context);
    }

    public interface IService<in TContext, TResult>
        where TContext : IServiceContext
    {
        Task<TResult> ExecuteAsync(TContext context);
        TResult Execute(TContext context);
    }
}
