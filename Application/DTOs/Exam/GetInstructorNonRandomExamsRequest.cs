namespace Application.DTOs.Exam
{
    public class GetInstructorNonRandomExamsRequest
    {
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid InstructorId { get; set; }
    }
}
