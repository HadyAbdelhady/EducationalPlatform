namespace Application.DTOs.Review
{
    public class GetReviewByIdResponse
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid EntityId { get; set; }
        public int StarRating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public StudentReviewInfo? Student { get; set; }
    }

    public class StudentReviewInfo
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PersonalPictureUrl { get; set; }
    }
}

