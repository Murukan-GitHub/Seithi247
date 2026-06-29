namespace Seithi247.Models
{
    public class CommentReaction
    {
        public int Id { get; set; }

        public int CommentId { get; set; }

        public string Emoji { get; set; }

        public string UserIdentifier { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual Comment Comment { get; set; }
    }
}
