﻿using CQRS.Core.Domains;
using CQRS.Core.Events;
using CQRS.Core.MongoDB.Config;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.MongoDB.Command.Infra.Repositories
{
    public abstract class EventStoreRepository<TId> : IEventStoreRepository<TId>
    {
        private readonly IMongoCollection<EventModel<TId>> _eventStoreCollection;

        public EventStoreRepository(IOptions<MongoDbConfig> config)
        {
            var mongoClient = new MongoClient(config.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);

            _eventStoreCollection = mongoDatabase.GetCollection<EventModel<TId>>(config.Value.Collection);
        }

        public async Task<List<EventModel<TId>>> FindAllAsync()
        {
            return await _eventStoreCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<EventModel<TId>>> FindByAggregateId(TId aggregateId)
        {
            if (aggregateId == null)
                return null;

            return await _eventStoreCollection.Find(x => x.AggregateIdentifier.Equals(aggregateId)).ToListAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(EventModel<TId> @event)
        {
            await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
        }
    }
}
