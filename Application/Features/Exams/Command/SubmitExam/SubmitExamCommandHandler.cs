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
            ExamModelAnswer? ExamModelAnswer = await CollectExamModelAnswer(request.Exam, cancellationToken);

            var userRepository = unitOfWork.Repository<User>();
            var studentExamResultRepository = unitOfWork.Repository<StudentExamResult>();
         
            var Student = await userRepository.GetByIdAsync(request.Student, cancellationToken);

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
                .FirstOrDefaultAsync(ser => ser.ExamId == request.Exam && ser.StudentId == request.Student, cancellationToken);

            if (examResult == null)
            {
                return Result<SubmissionResponse>.FailureStatusCode("Exam not found for this student", ErrorType.NotFound);
            }

            // Check if exam is in progress (student must have started the exam)
            if (examResult.Status != ExamResultStatus.InProgress)
            {
                return Result<SubmissionResponse>.FailureStatusCode(
                    examResult.Status == ExamResultStatus.NotStarted 
                        ? "Exam has not been started yet" 
                        : "Exam has already been submitted",
                    ErrorType.Conflict);
            }

            // Calculate marks
            var studentActualMark = CalculateObtainedMarks.Calculate(ExamModelAnswer, request);
            var studentPercentage = (studentActualMark / ExamModelAnswer.TotalMark) * 100;
            var isPassed = studentPercentage >= ExamModelAnswer.PassMarkPercentage;

            // Update the existing StudentExamResult
            examResult.StudentMark = studentActualMark;
            examResult.Status = isPassed ? ExamResultStatus.Passed : ExamResultStatus.Failed;
            examResult.UpdatedAt = DateTimeOffset.UtcNow;
            studentExamResultRepository.Update(examResult);

            // Add student answers
            foreach (var answer in request.Answers)
            {
                Domain.Entities.StudentAnswers submission = new()
                {
                    Id = Guid.NewGuid(),
                    QuestionId = answer.QuestionId,
                    ChosenAnswerId = answer.ChosenAnswerId,
                    ExamResultId = examResult.Id,
                    StudentId = request.Student
                };

                await unitOfWork.Repository<Domain.Entities.StudentAnswers>().AddAsync(submission, cancellationToken);
            }

            // Publish event to trigger any additional processing
            await mediator.Publish(new ExamFinishedEvent(request.Exam, request.Student, examResult.Id), cancellationToken);
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

        private async Task<ExamModelAnswer> CollectExamModelAnswer(Guid examId, CancellationToken cancellationToken)
        {
            var examRepository = unitOfWork.GetRepository<IExamRepository>();

            ExamModelAnswer? exam = await examRepository.GetExamWithQuestionsAndAnswersByIdAsync(examId, cancellationToken)
                                            ?? throw new Exception("Exam not found");

            var examModelAnswer = new ExamModelAnswer
            {
                ExamId = exam.ExamId,
                TotalMark = exam.TotalMark,
                Title = exam.Title,
                PassMarkPercentage = exam.PassMarkPercentage,
                Questions = [.. exam.Questions.Select(q => new QuestionModelAnswer
                {
                    QuestionId = q.QuestionId,
                    CorrectAnswerId = q.CorrectAnswerId,
                    QuestionMark = q.QuestionMark
                })]
            };
            return examModelAnswer;
        }
    }


}
