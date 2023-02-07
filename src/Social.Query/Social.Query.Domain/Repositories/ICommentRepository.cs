using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Social.Query.Domain.Entities;

namespace Social.Query.Domain.Repositories
{
    public interface ICommentRepository
    {
        Task CreateAsync(CommentEntity comment);
        Task<CommentEntity> GetByIdAsync(Guid commentId);
        Task UpdateAsync(CommentEntity comment);
        Task DeleteAsync(Guid commentId);
    }
}