using Seithi247.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Seithi247.Services
{
    public interface ICommentService
    {
        // Comment CRUD
        Task<Comment> AddCommentAsync(int newsId, string authorName, string text, int? parentCommentId = null);
        Task<Comment> GetCommentAsync(int commentId);
        Task<List<Comment>> GetCommentsForNewsAsync(int newsId);
        Task<List<Comment>> GetRepliesForCommentAsync(int parentCommentId);
        Task<Comment> UpdateCommentAsync(int commentId, string newText);
        Task<bool> DeleteCommentAsync(int commentId);

        // Reactions
        Task<CommentReaction> AddReactionAsync(int commentId, string userIdentifier, string emoji);
        Task<bool> RemoveReactionAsync(int commentId, string userIdentifier, string emoji);
        Task<Dictionary<string, int>> GetReactionSummaryAsync(int commentId);
        Task<List<CommentReaction>> GetReactionsForCommentAsync(int commentId);

        // Utilities
        Task<int> GetCommentCountAsync(int newsId);
    }
}
