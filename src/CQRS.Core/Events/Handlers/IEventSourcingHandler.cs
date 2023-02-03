using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domains; 


namespace CQRS.Core.Events.Handlers
{
    public interface IEventSourcingHandler<TAggregateRoot,TId> where TAggregateRoot : AggregateRoot<TId>
    {
        Task SaveAsync(TAggregateRoot aggregate);
        Task<TAggregateRoot> GetByIdAsync(TId aggregateId);
        Task RepublishEventsAsync();
    }
}