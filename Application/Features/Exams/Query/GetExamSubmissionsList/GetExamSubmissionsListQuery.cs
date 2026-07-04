using Application.Common;
using Application.Features.Exams.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Exams.Query.GetExamSubmissionsList
{
    public class GetExamSubmissionsListQuery : IRequest<Result<ExamSubmissionsListResponse>>
    {
        public Guid ExamId { get; set; }
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid InstructorId { get; set; }
    }
}
