namespace Application.DTOs.Exam
{
    public class GetAllExamsRequest
    {
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid UserId { get; set; }
    }
}
