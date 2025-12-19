using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Seithi247.Models
{
    public enum NewsType
    {
        Text = 0,
        Image = 1,
        Video = 2
    }
    public enum Country
    {

        Australia,
        USA,
        India,
        UK,
        SriLanka,
        Other
    }

    public enum NewsCategory
    {

        Cinema,
        Politics,
        Sports,
        Funny,
        Technology,
        Health,
        Home
    }
    public class News
    {
        public int Id { get; set; }
        [Required, StringLength(200)]
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Summary { get; set; }
        [DataType(DataType.Html)]
        public string Content { get; set; }
        [NotMapped]

        public string Editor { get; set; }
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        // optional: author, tags, isPublished flag
        public string Author { get; set; }
        public bool IsPublished { get; set; } = true;

        public Country Country { get; set; }         // enum property
        public NewsCategory NewsCategory { get; set; }  // enum property

        public NewsType NewsType { get; set; } = NewsType.Text;
        public virtual ICollection<NewsImage> Images { get; set; } = new List<NewsImage>();
        public virtual ICollection<NewsMedia> NewsMedias { get; set; } = new List<NewsMedia>();

        // Likes count
        public int Likes { get; set; }

        // Navigation property        
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>(); 

        [NotMapped]
        public string VideoUrl { get; set; }



    }
}
