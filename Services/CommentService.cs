using Seithi247.Data;
using Seithi247.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seithi247.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============ COMMENT CRUD ============

        public async Task<Comment> AddCommentAsync(int newsId, string authorName, string text, int? parentCommentId = null)
        {
            var comment = new Comment
            {
                NewsId = newsId,
                AuthorName = authorName,
                Text = text,
                PostedDate = DateTime.UtcNow,
                ParentCommentId = parentCommentId,
                IsDeleted = false,
                IsEdited = false
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> GetCommentAsync(int commentId)
        {
            return await _context.Comments
                .Include(c => c.Reactions)
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
        }

        public async Task<List<Comment>> GetCommentsForNewsAsync(int newsId)
        {
            return await _context.Comments
                .Where(c => c.NewsId == newsId && c.ParentCommentId == null && !c.IsDeleted)
                .Include(c => c.Reactions)
                .Include(c => c.Replies.Where(r => !r.IsDeleted))
                    .ThenInclude(r => r.Reactions)
                .OrderByDescending(c => c.PostedDate)
                .ToListAsync();
        }

        public async Task<List<Comment>> GetRepliesForCommentAsync(int parentCommentId)
        {
            return await _context.Comments
                .Where(c => c.ParentCommentId == parentCommentId && !c.IsDeleted)
                .Include(c => c.Reactions)
                .OrderBy(c => c.PostedDate)
                .ToListAsync();
        }

        public async Task<Comment> UpdateCommentAsync(int commentId, string newText)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || comment.IsDeleted)
                return null;

            comment.Text = newText;
            comment.EditedDate = DateTime.UtcNow;
            comment.IsEdited = true;

            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return false;

            comment.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============ REACTIONS ============

        public async Task<CommentReaction> AddReactionAsync(int commentId, string userIdentifier, string emoji)
        {
            // Check if reaction already exists
            var existingReaction = await _context.CommentReactions
                .FirstOrDefaultAsync(r => r.CommentId == commentId &&
                                          r.UserIdentifier == userIdentifier &&
                                          r.Emoji == emoji);

            if (existingReaction != null)
                return existingReaction; // Already reacted with this emoji

            var reaction = new CommentReaction
            {
                CommentId = commentId,
                UserIdentifier = userIdentifier,
                Emoji = emoji,
                ReactedDate = DateTime.UtcNow
            };

            _context.CommentReactions.Add(reaction);
            await _context.SaveChangesAsync();
            return reaction;
        }

        public async Task<bool> RemoveReactionAsync(int commentId, string userIdentifier, string emoji)
        {
            var reaction = await _context.CommentReactions
                .FirstOrDefaultAsync(r => r.CommentId == commentId &&
                                          r.UserIdentifier == userIdentifier &&
                                          r.Emoji == emoji);

            if (reaction == null)
                return false;

            _context.CommentReactions.Remove(reaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Dictionary<string, int>> GetReactionSummaryAsync(int commentId)
        {
            var reactions = await _context.CommentReactions
                .Where(r => r.CommentId == commentId)
                .GroupBy(r => r.Emoji)
                .Select(g => new { Emoji = g.Key, Count = g.Count() })
                .ToListAsync();

            return reactions.ToDictionary(r => r.Emoji, r => r.Count);
        }

        public async Task<List<CommentReaction>> GetReactionsForCommentAsync(int commentId)
        {
            return await _context.CommentReactions
                .Where(r => r.CommentId == commentId)
                .OrderByDescending(r => r.ReactedDate)
                .ToListAsync();
        }

        // ============ UTILITIES ============

        public async Task<int> GetCommentCountAsync(int newsId)
        {
            return await _context.Comments
                .CountAsync(c => c.NewsId == newsId && !c.IsDeleted);
        }
    }
}
