
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
using Social.Query.Infra.Converters;
using Social.Query.Infra.DataAccess;
using Social.Query.Infra.Dispatcher;
using Social.Query.Infra.Repositories;
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

builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));

// create database and tables
var dataContext = builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
dataContext.Database.EnsureCreated();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IQueryHandler, QueryHandler>();
//builder.Services.AddScoped<Social.Query.Infra.Handlers.ISocialEventTargetHandler, Social.Query.Infra.Handlers.SocialEventTargetHandler>();
builder.Services.AddScoped<IEventTargetHandler, Social.Query.Infra.Handlers.SocialEventTargetHandler>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddScoped<IEventConsumer, KafkaEventConsumer>();

builder.Services.AddSingleton<JsonConverter<BaseEvent>>(new EventJsonConverter());

// register query handler methods
var queryHandler = builder.Services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
var dispatcher = new SocialQueryDispatcher();
dispatcher.RegisterHandler<FindAllPostsQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsByAuthorQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsWithCommentsQuery>(queryHandler.HandleAsync);
dispatcher.RegisterHandler<FindPostsWithLikesQuery>(queryHandler.HandleAsync);
builder.Services.AddScoped<IQueryDispatcher<PostEntity>>(_ => dispatcher);

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
