using CQRS.Core.MongoDB.Command.Infra.Repositories;
using CQRS.Core.MongoDB.Config;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Command.Infra.Repositories
{
    public class SocialEventStoreRepository : EventStoreRepository<Guid>, ISocialEventStoreRepository
    {
        public SocialEventStoreRepository(IOptions<MongoDbConfig> config) : base(config)
        {
        }
    }
}
