using CQRS.Core.Domains;
using CQRS.Core.Events.Config;
using CQRS.Core.Events.Producers;
using CQRS.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Events.Infrastructures
{
    public class EventStore<TAggregateRoot,TId> : IEventStore<TAggregateRoot, TId> where TAggregateRoot: AggregateRoot<TId>
    {
        private readonly IEventStoreRepository<TId> eventStoreRepository;
        private readonly IEventProducer eventProducer;
        private readonly EventBusConfig eventBusConfig;

        public EventStore(EventBusConfig eventBusConfig, IEventStoreRepository<TId> eventStoreRepository, IEventProducer eventProducer)
        {
            this.eventStoreRepository = eventStoreRepository;
            this.eventProducer = eventProducer;
            this.eventBusConfig = eventBusConfig;
        }

        public async Task<List<TId>> GetAggregateIdsAsync()
        {
            var eventStream = await eventStoreRepository.FindAllAsync();

            if (eventStream == null || !eventStream.Any())
                throw new ArgumentNullException(nameof(eventStream), "Could not retrieve event stream from the event store!");

            return eventStream.Select(x => x.AggregateIdentifier).Distinct().ToList();
        }

        public async Task<List<BaseEvent>> GetEventsAsync(TId aggregateId)
        {
            var eventStream = await eventStoreRepository.FindByAggregateId(aggregateId);

            if (eventStream == null || !eventStream.Any())
                throw new AggregateNotFoundException("Incorrect post ID provided!");

            return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
        }

        public async Task SaveEventsAsync(TId aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await eventStoreRepository.FindByAggregateId(aggregateId);

            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
                throw new ConcurrencyException();

            var version = expectedVersion;

            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel<TId>
                {
                    TimeStamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = typeof(TAggregateRoot).Name,
                    Version = version,
                    EventType = eventType,
                    EventData = @event
                };

                await eventStoreRepository.SaveAsync(eventModel);

                await eventProducer.ProduceAsync(eventBusConfig.Topic, @event);
            }
        }
    }
}
