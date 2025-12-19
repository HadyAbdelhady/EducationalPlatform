using Application.Features.Question.Command.UpdateQuestion;
using Application.Interfaces;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Exam.Command.UpdateExam
{
    public class UpdateExamCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<UpdateExamCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;

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
                // Update questions
                foreach (var dto in request.ModifiedQuestions)
                {
                    Domain.Entities.ExamBank? questionLink = Exam.ExamQuestions.FirstOrDefault(eq => eq.QuestionId == dto.Id);
                    if (questionLink != null)
                    {
                        var q = questionLink.Question;
                        q.QuestionString = dto.QuestionText;
                        q.QuestionImageUrl = dto.ImageUrl;
                        questionLink.QuestionMark = dto.Mark;
                        q.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                    else
                    {
                        // new question link so i need to create it and link it to the exam
                        //await _mediator.Send(new UpdateQuestionCommand
                        //{
                        //    QuestionId = dto.Id,
                        //    QuestionString = dto.QuestionText,
                        //    QuestionImageUrl = dto.ImageUrl,
                        //    Answers = [.. dto.Answers],
                        //}, cancellationToken);
                    }
                }
            }
            if (request.ModifiedAnswerDto is not null && request.ModifiedAnswerDto.Count != 0)
            {
                // Update answers
                foreach (var answerDto in request.ModifiedAnswerDto)
                {
                    var answer = Exam.ExamQuestions
                                            .SelectMany(eq => eq.Question.Answers)
                                            .FirstOrDefault(a => a.Id == answerDto.Id);

                    if (answer != null)
                    {
                        answer.AnswerText = answerDto.AnswerText;
                        answer.IsCorrect = answerDto.IsCorrect;
                        answer.Explanation = answerDto.Explanation;
                        answer.UpdatedAt = DateTimeOffset.UtcNow;
                    }
                    else
                    {
                        // new answer was added so i need to create it and link it to the question
                    }
                }

            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
