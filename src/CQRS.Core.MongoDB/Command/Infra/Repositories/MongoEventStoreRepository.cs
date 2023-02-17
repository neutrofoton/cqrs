using CQRS.Core.Domains;
using CQRS.Core.Events;
using CQRS.Core.MongoDB.Config;
using CQRS.Core.MongoDB.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.MongoDB.Command.Infra.Repositories
{
    public class MongoEventStoreRepository<TId> : IEventStoreRepository<TId>
    {
        private readonly IMongoCollection<MongoEventModel<TId>> _eventStoreCollection;

        public MongoEventStoreRepository(IOptions<MongoDbConfig> config)
        {
            var mongoClient = new MongoClient(config.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);

            _eventStoreCollection = mongoDatabase.GetCollection<MongoEventModel<TId>>(config.Value.Collection);
        }

        public async Task<List<EventModel<TId>>> FindAllAsync()
        {
            var founds = await _eventStoreCollection
                .Find(_ => true)
                .ToListAsync()
                .ConfigureAwait(false);
                

            return founds.ConvertAll<EventModel<TId>>(x => x);
        }

        public async Task<List<EventModel<TId>>> FindByAggregateId(TId aggregateId)
        {
            if (aggregateId == null)
                return null;

            var founds = await _eventStoreCollection.Find(x => x.AggregateIdentifier.Equals(aggregateId)).ToListAsync().ConfigureAwait(false);
            return founds.ConvertAll<EventModel<TId>>(x => x);
        }

        public async Task SaveAsync(EventModel<TId> @event)
        {
            await _eventStoreCollection.InsertOneAsync(new MongoEventModel<TId>(@event)).ConfigureAwait(false);
        }
    }
}
