using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domains;
using CQRS.Core.Events.Config;
using CQRS.Core.Events.Infrastructures;
using CQRS.Core.Events.Producers;

namespace CQRS.Core.Events.Handlers
{
    public class EventSourcingHandler<TAggregateRoot,TId> : IEventSourcingHandler<TAggregateRoot, TId> where TAggregateRoot : AggregateRoot<TId>, new()
    {
        private readonly EventBusConfig eventBusConfig;
        private readonly IEventStore<TAggregateRoot,TId> eventStore;
        private readonly IEventProducer eventProducer;

        public EventSourcingHandler(EventBusConfig eventBusConfig, IEventStore<TAggregateRoot,TId> eventStore, IEventProducer eventProducer)
        {
            this.eventBusConfig = eventBusConfig;
            this.eventStore = eventStore;
            this.eventProducer = eventProducer;
        }

        public async Task<TAggregateRoot> GetByIdAsync(TId aggregateId)
        {
            var aggregate = new TAggregateRoot();
            var events = await eventStore.GetEventsAsync(aggregateId);

            if (events == null || !events.Any()) return aggregate;

            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(x => x.Version).Max();

            return aggregate;
        }

        public async Task RepublishEventsAsync()
        {
            var aggregateIds = await eventStore.GetAggregateIdsAsync();

            if (aggregateIds == null || !aggregateIds.Any()) return;

            foreach (var aggregateId in aggregateIds)
            {
                var aggregate = await GetByIdAsync(aggregateId);

                if (aggregate == null || !aggregate.Active) continue;

                var events = await eventStore.GetEventsAsync(aggregateId);

                foreach (var @event in events)
                {
                    await eventProducer.ProduceAsync(eventBusConfig.Topic, @event);
                }
            }
        }

        public async Task SaveAsync(TAggregateRoot aggregate)
        {
            await eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version);
            aggregate.MarkChangesAsCommitted();
        }
    }
}