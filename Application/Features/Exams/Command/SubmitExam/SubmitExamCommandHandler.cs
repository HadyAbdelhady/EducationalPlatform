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

            var studentExamResultRepository = unitOfWork.Repository<User>();
         
            var Student = await studentExamResultRepository.GetByIdAsync(request.Student, cancellationToken);

            if (Student == null)
            {
                return Result<SubmissionResponse>.FailureStatusCode("Student not found", ErrorType.NotFound);
            }

            if (ExamModelAnswer == null)
            {
                return Result<SubmissionResponse>.FailureStatusCode("Exam not found", ErrorType.NotFound);
            }

            var examResultId = Guid.NewGuid();
            
            StudentExamResult examResult = new()
            {
                Id = examResultId,
                StudentId = request.Student,
                ExamId = request.Exam,
                StudentMark = null, // Will be calculated in the event handler
            };

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

            await unitOfWork.Repository<StudentExamResult>().AddAsync(examResult, cancellationToken);

            // Publish event to trigger calculation in the event handler
            await mediator.Publish(new ExamFinishedEvent(request.Exam, request.Student, examResultId), cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Calculate marks for response (event handler will persist the result)
            var studentActualMark = CalculateObtainedMarks.Calculate(ExamModelAnswer, request);
            var studentPercentage = (studentActualMark / ExamModelAnswer.TotalMark) * 100;

            return Result<SubmissionResponse>.Success(new SubmissionResponse
            {
                StudentName = Student.FullName,
                ExamName = ExamModelAnswer.Title,
                TotalMark = ExamModelAnswer.TotalMark,
                ObtainedMark = studentActualMark,
                StatusMessage = $"Exam submitted successfully with {studentPercentage} % obtained.",
                IsSuccessful = studentPercentage >= ExamModelAnswer.PassMarkPercentage
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
