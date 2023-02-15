using Confluent.Kafka;
using CQRS.Core.Events.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CQRS.Core.Events.Handlers;
using System.Text.Json.Serialization;

namespace CQRS.Core.Kafka.Events.Consumers
{
    public class KafkaEventConsumer : EventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly JsonConverter<BaseEvent> _jsonConverter;
        public KafkaEventConsumer(
            IEventListenerHandler eventHandler,
            IOptions<ConsumerConfig> config,
            JsonConverter<BaseEvent> jsonConverter
            ) : base(eventHandler)
        {
            _config = config.Value;
            _jsonConverter = jsonConverter;
        }

        public override void Consume(string topic)
        {
            using var consumer = new ConsumerBuilder<string, string>(_config)
                    .SetKeyDeserializer(Deserializers.Utf8)
                    .SetValueDeserializer(Deserializers.Utf8)
                    .Build();

            consumer.Subscribe(topic);

            while (true)
            {
                var consumeResult = consumer.Consume();

                if (consumeResult?.Message == null) continue;

                var options = new JsonSerializerOptions { Converters = { _jsonConverter } };
                var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);
                var handlerMethod = EventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });

                if (handlerMethod == null)
                {
                    throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method!");
                }

                handlerMethod.Invoke(EventHandler, new object[] { @event });
                consumer.Commit(consumeResult);
            }
        }
    }
}
