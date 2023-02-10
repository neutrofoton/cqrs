using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Queries;
using CQRS.Core.Queries.Infrastructures;
using Social.Query.Domain.Entities;

namespace Social.Query.Infra.Dispatcher
{
    public class SocialQueryDispatcher : QueryDispatcher<PostEntity>
    {   
    }
}