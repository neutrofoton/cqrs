using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Events;

namespace CQRS.Core.Domains
{
    public interface IEventStoreRepository<TId>
    {
        Task SaveAsync(EventModel<TId> @event);
        Task<List<EventModel<TId>>> FindByAggregateId(TId aggregateId);
        Task<List<EventModel<TId>>> FindAllAsync();
    }
}