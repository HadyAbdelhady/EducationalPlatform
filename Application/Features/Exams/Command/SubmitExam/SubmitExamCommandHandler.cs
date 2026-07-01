using Application.HelperFunctions;
using Application.ResultWrapper;
using Application.Interfaces;
using Application.DTOs.Exam;
using Domain.Entities;
using Domain.enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Exams.Command.SubmitExam
{
    public class SubmitExamCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<SubmitExamCommand, Result<SubmissionResponse>>
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IMediator mediator = mediator;

        public async Task<Result<SubmissionResponse>> Handle(SubmitExamCommand request, CancellationToken cancellationToken)
        {
            ExamModelAnswer? ExamModelAnswer = await CollectExamModelAnswer(request.ExamId, cancellationToken);

            var userRepository = unitOfWork.Repository<User>();
            var studentExamResultRepository = unitOfWork.Repository<StudentExamResult>();

            var Student = await userRepository.GetByIdAsync(request.StudentId, cancellationToken);

            if (Student == null)
            {
                return Result<SubmissionResponse>.FailureStatusCode("Student not found", ErrorType.NotFound);
            }

            if (ExamModelAnswer == null)
            {
                return Result<SubmissionResponse>.FailureStatusCode("Exam not found", ErrorType.NotFound);
            }

            // Find the existing StudentExamResult (created during exam generation)
            var examResult = await studentExamResultRepository
                .FirstOrDefaultAsync(ser => ser.ExamId == request.ExamId && ser.StudentId == request.StudentId, cancellationToken);

            if (examResult == null)
            {
                return Result<SubmissionResponse>.FailureStatusCode("Exam has not been started yet", ErrorType.NotFound);
            }

            // Check if exam is in progress (student must have started the exam)
            if (examResult.Status == ExamResultStatus.InProgress)
            {
                return Result<SubmissionResponse>.FailureStatusCode("Exam has already been submitted", ErrorType.Conflict);
            }

            // Calculate marks
            var studentActualMark = CalculateObtainedMarks.Calculate(ExamModelAnswer, request);
            var studentPercentage = (studentActualMark / ExamModelAnswer.TotalMark) * 100;
            var isPassed = studentPercentage >= ExamModelAnswer.PassMarkPercentage;

            // Update the existing StudentExamResult
            examResult.StudentMark = studentActualMark;
            examResult.Status = isPassed ? ExamResultStatus.Passed : ExamResultStatus.Failed;
            examResult.UpdatedAt = EgyptTime.UtcNow;
            studentExamResultRepository.Update(examResult);

            // Add student answers — only for question IDs that belong to this exam
            var validQuestionIds = ExamModelAnswer.Questions.Select(q => q.QuestionId).ToHashSet();
            foreach (var answer in request.Answers.Where(a => validQuestionIds.Contains(a.QuestionId)))
            {
                examResult.StudentSubmissions.Add(new StudentAnswers
                {
                    QuestionId = answer.QuestionId,
                    ChosenAnswerId = answer.ChosenAnswerId,
                    ExamResultId = examResult.Id,
                    StudentId = request.StudentId
                });
            }

            // Publish event to trigger any additional processing
            await mediator.Publish(new ExamFinishedEvent(request.ExamId, request.StudentId, examResult.Id), cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<SubmissionResponse>.Success(new SubmissionResponse
            {
                StudentName = Student.FullName,
                ExamName = ExamModelAnswer.Title,
                TotalMark = ExamModelAnswer.TotalMark,
                ObtainedMark = studentActualMark,
                StatusMessage = $"Exam submitted successfully with {studentPercentage:F2}% obtained.",
                IsSuccessful = isPassed
            });
        }

        private async Task<ExamModelAnswer?> CollectExamModelAnswer(Guid examId, CancellationToken cancellationToken)
        {
            var examRepository = unitOfWork.GetRepository<IExamRepository>();
            return await examRepository.GetExamWithQuestionsAndAnswersByIdAsync(examId, cancellationToken);
        }
    }


}
