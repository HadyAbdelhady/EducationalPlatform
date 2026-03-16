using Application.DTOs.Exam;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exams.Query.GetStudentsSubmittionsForExam
{
    public class GetStudentsSubmittionsForExamQueryHandler : IRequestHandler<GetStudentsSubmittionsForExamQuery, Result<PaginatedResult<ExamListDto>>>
    {
        public Task<Result<PaginatedResult<ExamListDto>>> Handle(GetStudentsSubmittionsForExamQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
