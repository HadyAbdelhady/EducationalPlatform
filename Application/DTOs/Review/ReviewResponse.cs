namespace Application.DTOs.Review
{
    public class ReviewResponse
    {
        public Guid CourseReviewId { get; set; }
        public int StarRating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
