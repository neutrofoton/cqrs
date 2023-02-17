using CQRS.Core.Events.Config;
using CQRS.Core.Events.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Kafka.Events.Consumers
{
    public class ConsumerHostedService : IHostedService
    {
        private readonly ILogger<ConsumerHostedService> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly EventBusConfig eventBusConfig;

        public ConsumerHostedService(EventBusConfig eventBusConfig, ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.eventBusConfig= eventBusConfig;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Event Consumer Service running.");

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                Task.Run(() => eventConsumer.Consume(eventBusConfig.Topic), cancellationToken);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Event Consumer Service Stopped");

            return Task.CompletedTask;
        }
    }
}
