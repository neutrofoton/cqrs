using CQRS.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Social.Command.Infra.Repositories
{
    public interface ISocialEventStoreRepository : IEventStoreRepository<Guid>
    {
    }
}
