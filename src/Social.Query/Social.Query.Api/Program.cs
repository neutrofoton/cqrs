
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Events.Config;
using CQRS.Core.Events.Consumers;
using CQRS.Core.Events.Handlers;
using CQRS.Core.Kafka.Events.Consumers;
using CQRS.Core.Queries.Infrastructures;
using Microsoft.EntityFrameworkCore;
using Social.Query.Api.Handlers;
using Social.Query.Api.Queries;
using Social.Query.Domain.Entities;
using Social.Query.Domain.Repositories;
using Social.Query.Infra.DataAccess;
using Social.Query.Infra.Handlers;
using Social.Query.Infra.Repositories;
using Social.Shared.Converters;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Action<DbContextOptionsBuilder> configureDbContext;
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

if (env.Equals("Development.PostgreSQL"))
{
    configureDbContext = o => o.UseLazyLoadingProxies().UseNpgsql(builder.Configuration.GetConnectionString("SqlServer"));
}
else
{
    string sqlConn = builder.Configuration.GetConnectionString("SqlServer");
    configureDbContext = o => o.UseLazyLoadingProxies().UseSqlServer(sqlConn);
}

builder.Services.AddSingleton(serviceProvider =>
{
    var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
    return new EventBusConfig()
    {
        Topic = topic,
    };
});

builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddSingleton<JsonConverter<EventMessage>>(new EventJsonConverter());

//database configuration
builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));

var dataContext = builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
dataContext.Database.EnsureCreated();// create database and tables

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();


builder.Services.AddScoped<IEventListenerHandler, SocialEventListenerHandler>();
builder.Services.AddScoped<IEventConsumer, KafkaEventConsumer>();

// register query handler methods
builder.Services.AddScoped<IQueryHandler, QueryHandler>();
var queryHandler = builder.Services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
builder.Services.AddScoped<IQueryDispatcher<PostEntity>>(sp =>
{  
    var dispatcher = new QueryDispatcher<PostEntity>();
    dispatcher.RegisterHandler<FindAllPostsQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostsByAuthorQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostsWithCommentsQuery>(queryHandler.HandleAsync);
    dispatcher.RegisterHandler<FindPostsWithLikesQuery>(queryHandler.HandleAsync);

    return dispatcher;
});

builder.Services.AddControllers();
builder.Services.AddHostedService<ConsumerHostedService>();

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
