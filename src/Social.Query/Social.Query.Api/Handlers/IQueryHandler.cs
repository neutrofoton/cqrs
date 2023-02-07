using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Social.Query.Domain.Entities;
using Social.Query.Api.Queries;

namespace Social.Query.Api.Handlers
{
    public interface IQueryHandler
    {
        Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query);
        Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query);
    }
}