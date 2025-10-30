using Seithi247.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Seithi247.Views.ViewModel
{
    public class NewsVM
    {

        public int Id { get; set; }
        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }
        public DateTime PublishedAt { get; set; } 
        // optional: author, tags, isPublished flag
        public string Author { get; set; }
        public bool IsPublished { get; set; } 

        public Country Country { get; set; }         // enum property
        public NewsCategory NewsCategory { get; set; }  // enum property

        public NewsType NewsType { get; set; }
        public virtual ICollection<NewsImage> Images { get; set; } = new List<NewsImage>();
        public virtual ICollection<NewsMedia> NewsMedias { get; set; } = new List<NewsMedia>();

        // Likes count
        public int Likes { get; set; }

        // Navigation property        
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();


    }
}
