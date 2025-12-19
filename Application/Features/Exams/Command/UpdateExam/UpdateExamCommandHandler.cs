using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Exams.Command.UpdateExam
{
    public class UpdateExamCommandHandler(IUnitOfWork unitOfWork, IQuestionUpdateService questionUpdateService) : IRequestHandler<UpdateExamCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IQuestionUpdateService _questionUpdateService = questionUpdateService;

        public async Task<Result<bool>> Handle(UpdateExamCommand request, CancellationToken cancellationToken)
        {
            var examRepository = _unitOfWork.GetRepository<IExamRepository>();
            var Exam = await examRepository.GetExamEntityByIdAsync(request.ExamId, cancellationToken)
                                                ?? throw new ArgumentException("Exam not found.");

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
                // Update questions using the reusable service
                foreach (var dto in request.ModifiedQuestions)
                {
                    ExamBank? questionLink = Exam.ExamQuestions.FirstOrDefault(eq => eq.QuestionId == dto.Id);
                    if (questionLink != null)
                    {
                        var q = questionLink.Question;
                        
                        // Use the service to update question properties and manage answers
                        _questionUpdateService.UpdateQuestion(
                            q,
                            dto.QuestionText,
                            dto.ImageUrl,
                            [.. dto.Answers]);
                        
                        // Update the exam-specific question mark
                        questionLink.QuestionMark = dto.Mark;
                    }
                    else
                    {
                        // new question link so i need to create it and link it to the exam
                        // This case would require additional logic to create the question and link it
                    }
                }
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
