using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domains;
using CQRS.Core.Events;
using CQRS.Core.Events.Config;
using CQRS.Core.Events.Infrastructures;
using CQRS.Core.Events.Producers;
using CQRS.Core.Exceptions;
using Social.Command.Domain.Aggregates;
using Social.Command.Infra.Repositories;

namespace Social.Command.Infra.Stores
{
    public class SocialEventStore : EventStore<PostAggregate, Guid>, IEventStore<PostAggregate, Guid>
    {

        public SocialEventStore(EventBusConfig eventBusConfig, ISocialEventStoreRepository eventStoreRepository, IEventProducer eventProducer) : base(eventBusConfig, eventStoreRepository, eventProducer) { }

    }
}