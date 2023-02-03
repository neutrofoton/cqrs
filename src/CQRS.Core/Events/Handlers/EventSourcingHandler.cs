using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domains;
using CQRS.Core.Events.Infrastructures;
using CQRS.Core.Events.Producers;

namespace CQRS.Core.Events.Handlers
{
    public class EventSourcingHandler<TAggregateRoot,TId> : IEventSourcingHandler<TAggregateRoot, TId> where TAggregateRoot : AggregateRoot<TId>, new()
    {
        private readonly IEventStore<TAggregateRoot,TId> _eventStore;
        private readonly IEventProducer _eventProducer;

        public EventSourcingHandler(IEventStore<TAggregateRoot,TId> eventStore, IEventProducer eventProducer)
        {
            _eventStore = eventStore;
            _eventProducer = eventProducer;
        }

        public async Task<TAggregateRoot> GetByIdAsync(TId aggregateId)
        {
            var aggregate = new TAggregateRoot();
            var events = await _eventStore.GetEventsAsync(aggregateId);

            if (events == null || !events.Any()) return aggregate;

            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(x => x.Version).Max();

            return aggregate;
        }

        public async Task RepublishEventsAsync()
        {
            var aggregateIds = await _eventStore.GetAggregateIdsAsync();

            if (aggregateIds == null || !aggregateIds.Any()) return;

            foreach (var aggregateId in aggregateIds)
            {
                var aggregate = await GetByIdAsync(aggregateId);

                if (aggregate == null || !aggregate.Active) continue;

                var events = await _eventStore.GetEventsAsync(aggregateId);

                foreach (var @event in events)
                {
                    var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                    await _eventProducer.ProduceAsync(topic, @event);
                }
            }
        }

        public async Task SaveAsync(TAggregateRoot aggregate)
        {
            await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommittedChanges(), aggregate.Version);
            aggregate.MarkChangesAsCommitted();
        }
    }
}