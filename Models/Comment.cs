namespace Seithi247.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int NewsId { get; set; }
        public string AuthorName { get; set; }
        public string Text { get; set; }
        public DateTime PostedDate { get; set; }

        public News News { get; set; }
        public ICollection<CommentReaction> Reactions { get; set; } = new List<CommentReaction>();

    }
}
