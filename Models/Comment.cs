namespace Seithi247.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int NewsId { get; set; }

        public int? ParentCommentId { get; set; }

        public string AuthorName { get; set; }

        public string Text { get; set; }

        public DateTime PostedDate { get; set; }

        public bool IsEdited { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Comment ParentComment { get; set; }

        public virtual ICollection<Comment> Replies { get; set; }

        public virtual ICollection<CommentReaction> Reactions { get; set; }

    }
}
