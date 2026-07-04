using Application.Common;
namespace Application.Features.Exams.DTOs
{
    public class GetStudentsSubmittionsForExamRequest
    {
        public Guid ExamId { get; set; }
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
    }
}
