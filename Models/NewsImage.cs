using System.ComponentModel.DataAnnotations;

namespace Seithi247.Models
{
    public class NewsImage
    {
        public int Id { get; set; }

        [Required]
        public string FilePath { get; set; }

        public int NewsId { get; set; }
        public News News { get; set; }
    }
}
