namespace Seithi247.Models
{
    public class NewsRecommendation
    {
        public int Id { get; set; }
        public int NewsId { get; set; }
        public string RecommendedBy { get; set; } // userId / IP / session
        public DateTime RecommendedAt { get; set; }
    }
}
