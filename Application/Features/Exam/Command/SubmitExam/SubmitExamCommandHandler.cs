using Application.DTOs.Exam;
using Application.HelperFunctions;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Exam.Command.SubmitExam
{
    public class SubmitExamCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<SubmitExamCommand, Result<SubmissionResponse>>
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IMediator mediator = mediator;

        public async Task<Result<SubmissionResponse>> Handle(SubmitExamCommand request, CancellationToken cancellationToken)
        {
            ExamModelAnswer? ExamModelAnswer = await CollectExamModelAnswer(request.Exam, cancellationToken);

            if (ExamModelAnswer == null)
            {
                return Result<SubmissionResponse>.FailureStatusCode("Exam not found", ErrorType.NotFound);
            }

            StudentExamResult examResult = new()
            {
                Id = Guid.NewGuid(),
                StudentId = request.Student,
                ExamId = request.Exam,
                Status = ExamStatus.Submitted,
                StudentMark = CalculateObtainedMarks.Calculate(ExamModelAnswer, request),
            };

            foreach (var answer in request.Answers)
            {
                StudentSubmission submission = new()
                {
                    Id = Guid.NewGuid(),
                    QuestionId = answer.QuestionId,
                    ChosenAnswerId = answer.ChosenAnswerId,
                    ExamResultId = examResult.Id
                };

                await unitOfWork.Repository<StudentSubmission>().AddAsync(submission, cancellationToken);
            }

            await unitOfWork.Repository<StudentExamResult>().AddAsync(examResult, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var StudentActualMark = CalculateObtainedMarks.Calculate(ExamModelAnswer, request);
            var StudentPercentage = (StudentActualMark / ExamModelAnswer.TotalMark) * 100;

            return Result<SubmissionResponse>.Success(new SubmissionResponse
            {
                StudentName = request.Student,
                ExamName = request.Exam,
                TotalMark = ExamModelAnswer.TotalMark,
                ObtainedMark = StudentActualMark,
                StatusMessage = $"Exam submitted successfully with {StudentPercentage} % obtained.",
                IsSuccessful = StudentPercentage >= ExamModelAnswer.PassMarkPercentage
            });
        }

        private async Task<ExamModelAnswer> CollectExamModelAnswer(Guid examId, CancellationToken cancellationToken)
        {
            var examRepository = unitOfWork.GetRepository<IExamRepository>();

            ExamModelAnswer? exam = await examRepository.GetExamWithQuestionsAndAnswersByIdAsync(examId, cancellationToken)
                                            ?? throw new Exception("Exam not found");

            var examModelAnswer = new ExamModelAnswer
            {
                ExamId = exam.ExamId,
                Questions = [.. exam.Questions.Select(q => new QuestionModelAnswer
                {
                    QuestionId = q.QuestionId,
                    CorrectAnswerId = q.CorrectAnswerId
                })]
            };
            return examModelAnswer;
        }
    }


}
