using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Seithi247.Models
{

    public class NewsMedia
    {
        public int Id { get; set; }

        [Required]
        public int NewsId { get; set; }

        [ForeignKey("NewsId")]
        public News News { get; set; }

        [Required]
        [StringLength(500)]
        public string MediaUrl { get; set; }  // Video URL or Image URL

        [StringLength(500)]
        public string ThumbnailUrl { get; set; } // For video or image preview

        public bool IsVideo { get; set; } = false; // true if this media is a video
    }
}
