using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domains; 


namespace CQRS.Core.Events.Handlers
{
    public interface IEventSourcingHandler<T>
    {
        Task SaveAsync(AggregateRoot aggregate);
        Task<T> GetByIdAsync(Guid aggregateId);
        Task RepublishEventsAsync();
    }
}