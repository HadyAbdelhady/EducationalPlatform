namespace Application.DTOs.Exam
{
    public class GetInstructorExamsRequest
    {
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid InstructorId { get; set; }
    }
}
