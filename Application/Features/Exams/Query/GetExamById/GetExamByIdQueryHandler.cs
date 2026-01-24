using Application.DTOs.Exam;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Exams.Query.GetExamById
{
    public class GetExamByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetExamByIdQuery, Result<ExamDetailsQueryModel>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<ExamDetailsQueryModel>> Handle(GetExamByIdQuery request, CancellationToken cancellationToken)
        {
            var ExamRepository = _unitOfWork.GetRepository<IExamRepository>();
            var Exam = await ExamRepository.GetExamByIdWithQuestionsAndAnswersAsync(request.Id, cancellationToken);

            if (Exam == null)
                return Result<ExamDetailsQueryModel>.FailureStatusCode("Exam not found.", ErrorType.NotFound);

            return Result<ExamDetailsQueryModel>.Success(Exam);
        }
    }
}
