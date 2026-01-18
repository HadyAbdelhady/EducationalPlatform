using Application.DTOs;
using Application.DTOs.Exam;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exams.Query.Get_Exam_List
{
    public class GetAllExamsQuery : IRequest<Result<PaginatedResult<ExamListDto>>>
    {
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid UserId { get; set; }
    }



}