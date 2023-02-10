using Confluent.Kafka;
using CQRS.Core.Domains;
using CQRS.Core.Events;
using CQRS.Core.Events.Config;
using CQRS.Core.Events.Handlers;
using CQRS.Core.Events.Infrastructures;
using CQRS.Core.Events.Producers;
using CQRS.Core.Kafka.Events.Producers;
using CQRS.Core.MongoDB.Config;
using MongoDB.Bson.Serialization;
using Social.Command.Api.Handlers;
using Social.Command.Domain.Aggregates;
using Social.Command.Infra.Repositories;
using Social.Command.Infra.Stores;
using Social.Shared.Events;

var builder = WebApplication.CreateBuilder(args);


BsonClassMap.RegisterClassMap<BaseEvent>();
BsonClassMap.RegisterClassMap<PostCreatedEvent>();
BsonClassMap.RegisterClassMap<MessageUpdatedEvent>();
BsonClassMap.RegisterClassMap<PostLikedEvent>();
BsonClassMap.RegisterClassMap<CommentAddedEvent>();
BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
BsonClassMap.RegisterClassMap<CommentRemovedEvent>();
BsonClassMap.RegisterClassMap<PostRemovedEvent>();

// Add services to the container.
builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
builder.Services.AddSingleton(serviceProvider =>
{
    var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
    return new EventBusConfig()
    {
        Topic = topic,
    };
});

builder.Services.AddScoped<ISocialEventStoreRepository, SocialEventStoreRepository>();
builder.Services.AddScoped<IEventProducer, KafkaEventProducer>();
builder.Services.AddScoped<IEventStore<PostAggregate,Guid>, SocialEventStore>();
builder.Services.AddScoped<IEventSourcingHandler<PostAggregate,Guid>, EventSourcingHandler<PostAggregate,Guid>>();
builder.Services.AddScoped<ICommandHandler, CommandHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
