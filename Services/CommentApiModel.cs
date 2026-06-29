using System;
using System.Collections.Generic;

namespace Seithi247.Services
{
    // DTOs for AJAX API responses
    public class CommentApiDto
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string Text { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? EditedDate { get; set; }
        public bool IsEdited { get; set; }
        public int Likes { get; set; }
        public int? ParentCommentId { get; set; }
        public Dictionary<string, int> ReactionSummary { get; set; } = new();
        public List<CommentApiDto> Replies { get; set; } = new();
    }

    public class PostCommentRequest
    {
        public int NewsId { get; set; }
        public string AuthorName { get; set; }
        public string Text { get; set; }
        public int? ParentCommentId { get; set; }
    }

    public class DeleteCommentRequest
    {
        public int CommentId { get; set; }
    }

    public class EditCommentRequest
    {
        public int CommentId { get; set; }
        public string NewText { get; set; }
    }

    public class ReactionRequest
    {
        public int CommentId { get; set; }
        public string Emoji { get; set; }
    }
}
