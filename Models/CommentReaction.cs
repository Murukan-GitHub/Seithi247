namespace Seithi247.Models
{
    public class CommentReaction
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public string Emoji { get; set; }
        public string UserIdentifier { get; set; } // optional for logged-in users or anonymous tracking
        public DateTime ReactedOn { get; set; }

        public Comment Comment { get; set; }
    }
}
