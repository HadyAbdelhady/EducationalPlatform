using Application.Common;
namespace Application.Features.Exams.DTOs
{
    public class GetInstructorExamsRequest
    {
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid InstructorId { get; set; }
    }
}
