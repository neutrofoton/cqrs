using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Queries.Infrastructures
{
    public abstract class QueryDispatcher<TEntity> : IQueryDispatcher<TEntity>
    {
        private readonly Dictionary<Type, Func<BaseQuery, Task<List<TEntity>>>> _handlers = new();

        public void RegisterHandler<TQuery>(Func<TQuery, Task<List<TEntity>>> handler) where TQuery : BaseQuery
        {
            if (_handlers.ContainsKey(typeof(TQuery)))
            {
                throw new IndexOutOfRangeException("You cannot register the same query handler twice!");
            }

            _handlers.Add(typeof(TQuery), x => handler((TQuery)x));
        }

        public async Task<List<TEntity>> SendAsync(BaseQuery query)
        {
            if (_handlers.TryGetValue(query.GetType(), out Func<BaseQuery, Task<List<TEntity>>> handler))
            {
                return await handler(query);
            }

            throw new ArgumentNullException(nameof(handler), "No query handler was registered!");
        }
    }
}
