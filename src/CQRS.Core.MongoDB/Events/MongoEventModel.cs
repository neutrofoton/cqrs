using CQRS.Core.Events;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.MongoDB.Events
{
    public class MongoEventModel<TId> : EventModel<TId>
    {
        public MongoEventModel(EventModel<TId> eventModel)
        {
            this.TimeStamp = eventModel.TimeStamp;
            this.AggregateIdentifier = eventModel.AggregateIdentifier;
            this.AggregateType = eventModel.AggregateType;
            this.Version = eventModel.Version;
            this.EventType = eventModel.EventType;
            this.EventData = eventModel.EventData;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

    }
}
