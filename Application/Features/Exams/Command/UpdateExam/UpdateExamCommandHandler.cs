using Application.DTOs.Questions;
using Application.ResultWrapper;
using Application.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Exams.Command.UpdateExam
{
    public class UpdateExamCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateExamCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<bool>> Handle(UpdateExamCommand request, CancellationToken cancellationToken)
        {
            var examRepository = _unitOfWork.GetRepository<IExamRepository>();
            var Exam = await examRepository.GetExamEntityByIdAsync(request.ExamId, cancellationToken)
                                                ?? throw new ArgumentException("Exam not found.");

            var DeletedQuestions = Exam.ExamQuestions
                                                    .Where(eq => request.ModifiedQuestions.All(mq => mq.Id != eq.QuestionId))
                                                    .ToList();

            if (Exam.StartTime <= DateTimeOffset.Now)
            {
                return Result<bool>.FailureStatusCode("Sorry, the exam has already started.", ErrorType.BadRequest);
            }

            if (request.Title is not null)
            {
                Exam.Name = request.Title;
            }
            if (request.Description is not null)
            {
                Exam.Description = request.Description;
            }
            if (request.ScheduledDate is not null)
            {
                Exam.StartTime = request.ScheduledDate;
            }
            if (request.DurationInMinutes is not null)
            {
                Exam.DurationInMinutes = request.DurationInMinutes;
            }
            if (request.TotalMark is not null)
            {
                Exam.TotalMark = (decimal)request.TotalMark;
            }
            if (request.NumberOfQuestions is not null)
            {
                Exam.NumberOfQuestions = (int)request.NumberOfQuestions;
            }
            if (request.PassMarkPercentage is not null)
            {
                Exam.PassMarkPercentage = (int)request.PassMarkPercentage;
            }
           
            if (request.ModifiedQuestions is not null && request.ModifiedQuestions.Count != 0)
            {
                foreach (ModifiedQuestionsDto NewQuestionsList in request.ModifiedQuestions)
                {
                    var questionLink = Exam.ExamQuestions.Any(x => x.QuestionId == NewQuestionsList.Id);

                    if (!questionLink)
                    {
                        Exam.ExamQuestions.Add(new ExamBank
                        {
                            ExamId = Exam.Id,
                            QuestionId = NewQuestionsList.Id,
                            QuestionMark = NewQuestionsList.Mark
                        });
                    }
                }
                foreach (var deletedQuestion in DeletedQuestions)
                {
                    Exam.ExamQuestions.Remove(deletedQuestion);
                }
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
