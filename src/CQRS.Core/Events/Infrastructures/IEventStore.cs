using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domains;
using CQRS.Core.Events;

namespace CQRS.Core.Events.Infrastructures
{
    public interface IEventStore<TAggregateRoot,TId> where TAggregateRoot : AggregateRoot<TId>
    {
        Task SaveEventsAsync(TId aggregateId, IEnumerable<BaseEvent> events, int expectedVersion);
        Task<List<BaseEvent>> GetEventsAsync(TId aggregateId);
        Task<List<TId>> GetAggregateIdsAsync();
    }
}