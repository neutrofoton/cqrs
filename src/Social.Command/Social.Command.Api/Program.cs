using Confluent.Kafka;
using CQRS.Core.Commands.Infrastructures;
using CQRS.Core.Domains;
using CQRS.Core.Events;
using CQRS.Core.Events.Config;
using CQRS.Core.Events.Handlers;
using CQRS.Core.Events.Infrastructures;
using CQRS.Core.Events.Producers;
using CQRS.Core.Kafka.Events.Producers;
using CQRS.Core.MongoDB.Command.Infra.Repositories;
using CQRS.Core.MongoDB.Config;
using MongoDB.Bson.Serialization;
using Social.Command.Api.Commands;
using Social.Command.Api.Handlers;
using Social.Command.Domain.Aggregates;
using Social.Shared.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(serviceProvider =>
{
    var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
    return new EventBusConfig()
    {
        Topic = topic,
    };
});


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

//mongo event store
builder.Services.AddScoped<IEventStoreRepository<Guid>, MongoEventStoreRepository<Guid>>();
//kafka event bus.
builder.Services.AddScoped<IEventProducer, KafkaEventProducer>();

builder.Services.AddScoped<IEventStore<PostAggregate, Guid>, EventStore<PostAggregate, Guid>>();
builder.Services.AddScoped<IEventSourcingHandler<PostAggregate,Guid>, EventSourcingHandler<PostAggregate,Guid>>();



// register command handler methods
builder.Services.AddScoped<ICommandHandler, CommandHandler>();
var commandHandler = builder.Services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
builder.Services.AddSingleton<ICommandDispatcher>((sp) =>
{
    //var commandHandler = sp.GetRequiredService<ICommandHandler>();
    var dispatcher = new CommandDispatcher();

    dispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<EditMessageCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);
    dispatcher.RegisterHandler<RestoreReadDbCommand>(commandHandler.HandleAsync);

    return dispatcher;
});

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
