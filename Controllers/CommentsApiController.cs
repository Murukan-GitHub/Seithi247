using Seithi247.Services;
using Seithi247.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seithi247.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsApiController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsApiController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // ============ GET ENDPOINTS ============

        [HttpGet("news/{newsId}")]
        public async Task<IActionResult> GetCommentsForNews(int newsId)
        {
            var comments = await _commentService.GetCommentsForNewsAsync(newsId);
            var dtos = comments.Select(c => MapToDto(c)).ToList();
            return Ok(dtos);
        }

        [HttpGet("{commentId}")]
        public async Task<IActionResult> GetComment(int commentId)
        {
            var comment = await _commentService.GetCommentAsync(commentId);
            if (comment == null)
                return NotFound();

            return Ok(MapToDto(comment));
        }

        [HttpGet("replies/{parentCommentId}")]
        public async Task<IActionResult> GetReplies(int parentCommentId)
        {
            var replies = await _commentService.GetRepliesForCommentAsync(parentCommentId);
            var dtos = replies.Select(r => MapToDto(r)).ToList();
            return Ok(dtos);
        }

        [HttpGet("reactions/{commentId}")]
        public async Task<IActionResult> GetReactionSummary(int commentId)
        {
            var summary = await _commentService.GetReactionSummaryAsync(commentId);
            return Ok(summary);
        }

        // ============ POST ENDPOINTS ============

        [HttpPost("add")]
        public async Task<IActionResult> AddComment([FromBody] PostCommentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var comment = await _commentService.AddCommentAsync(
                request.NewsId,
                request.AuthorName,
                request.Text,
                request.ParentCommentId);

            return CreatedAtAction(nameof(GetComment), new { commentId = comment.Id }, MapToDto(comment));
        }

        [HttpPost("react")]
        public async Task<IActionResult> ReactToComment([FromBody] ReactionRequest request)
        {
            // Get user identifier (IP address or user ID)
            var userIdentifier = HttpContext.Connection.RemoteIpAddress?.ToString();

            var reaction = await _commentService.AddReactionAsync(
                request.CommentId,
                userIdentifier,
                request.Emoji);

            var summary = await _commentService.GetReactionSummaryAsync(request.CommentId);
            return Ok(new { success = true, reactionSummary = summary });
        }

        [HttpPost("unreact")]
        public async Task<IActionResult> UnreactToComment([FromBody] ReactionRequest request)
        {
            var userIdentifier = HttpContext.Connection.RemoteIpAddress?.ToString();

            var success = await _commentService.RemoveReactionAsync(
                request.CommentId,
                userIdentifier,
                request.Emoji);

            if (!success)
                return NotFound(new { success = false, message = "Reaction not found" });

            var summary = await _commentService.GetReactionSummaryAsync(request.CommentId);
            return Ok(new { success = true, reactionSummary = summary });
        }

        // ============ PUT ENDPOINTS ============

        [HttpPut("edit")]
        public async Task<IActionResult> EditComment([FromBody] EditCommentRequest request)
        {
            var comment = await _commentService.UpdateCommentAsync(request.CommentId, request.NewText);
            if (comment == null)
                return NotFound(new { success = false, message = "Comment not found" });

            return Ok(new { success = true, comment = MapToDto(comment) });
        }

        // ============ DELETE ENDPOINTS ============

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var success = await _commentService.DeleteCommentAsync(commentId);
            if (!success)
                return NotFound(new { success = false, message = "Comment not found" });

            return Ok(new { success = true, message = "Comment deleted successfully" });
        }

        // ============ HELPER METHODS ============

        private CommentApiDto MapToDto(Comment comment)
        {
            var reactionSummary = _commentService.GetReactionSummaryAsync(comment.Id).Result;

            return new CommentApiDto
            {
                Id = comment.Id,
                AuthorName = comment.AuthorName,
                Text = comment.Text,
                PostedDate = comment.PostedDate,
                EditedDate = comment.EditedDate,
                IsEdited = comment.IsEdited,
                Likes = comment.Likes,
                ParentCommentId = comment.ParentCommentId,
                ReactionSummary = reactionSummary,
                Replies = comment.Replies?
                    .Where(r => !r.IsDeleted)
                    .Select(r => MapToDto(r))
                    .ToList() ?? new List<CommentApiDto>()
            };
        }
    }
}
