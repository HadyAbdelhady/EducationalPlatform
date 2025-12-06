namespace Application.DTOs.Review
{
    public class ReviewResponse
    {
        public Guid ReviewId { get; set; }
        public int StarRating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
