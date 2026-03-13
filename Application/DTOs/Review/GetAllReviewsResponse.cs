namespace Application.DTOs.Review
{
    public class GetAllReviewsResponse
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public decimal StarRating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public StudentReviewInfo? Student { get; set; }
    }
}
