using Application.DTOs.Exam;
using Application.ResultWrapper;
using Application.Interfaces;
using Domain.Entities;
using Domain.enums;
using Domain.Events;
using MediatR;
using Application.HelperFunctions;

namespace Application.Features.Exams.Command.GenerateExam
{
    public class GenerateExamCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<GenerateExamCommand, Result<GenerateExamResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;


        public async Task<Result<GenerateExamResponse>> Handle(GenerateExamCommand request, CancellationToken cancellationToken)
        {
            var QuestionRepository = _unitOfWork.Repository<Question>();


            var question = QuestionRepository.Find(q => q.CourseId == request.CourseId &&
                                                                                        (!request.SectionId.HasValue || q.SectionId == request.SectionId)
                                                                                        , cancellationToken);

            if (!question.Any())
            {
                return Result<GenerateExamResponse>.FailureStatusCode("Question does not exist.", ErrorType.NotFound);
            }

            if (question.Count() < request.NumberOfQuestions)
            {
                return Result<GenerateExamResponse>.FailureStatusCode(
                    $"Not enough questions available. Requested: {request.NumberOfQuestions}, Available: {question.Count()}.",
                    ErrorType.BadRequest
                );
            }

            var examStartUtc = request.ExamStartTime.ToUniversalTime();
            var examEndUtc = request.ExamEndTime.ToUniversalTime();

            ExamStatus examStatus = ExamStatus.Draft;
            if (examStartUtc <= EgyptTime.UtcNow && examEndUtc >= EgyptTime.UtcNow)
            {
                return Result<GenerateExamResponse>.FailureStatusCode("Start time must be in the future.", ErrorType.BadRequest);
            }
            else if (examStartUtc > EgyptTime.UtcNow)
            {
                examStatus = ExamStatus.Scheduled;
            }

            Exam newExam = new()
            {
                Id = Guid.NewGuid(),
                CourseId = request.CourseId,
                SectionId = request.SectionId,
                Name = request.Title,
                Description = request.Description,
                InstructorId = request.CreatedBy,
                TotalMark = request.ExamTotalMark,
                NumberOfQuestions = request.NumberOfQuestions,
                StartTime = examStartUtc,
                EndTime = examEndUtc,
                ExamType = request.ExamType,
                IsRandomized = request.IsRandomized,
                DurationInMinutes = request.DurationInMinutes,
                PassMarkPercentage = request.PassMarkPercentage,
                Status = examStatus,
            };

            if (request.IsRandomized)
            {
                question.ToList().Shuffle();
                decimal markPerQuestion = request.ExamTotalMark / request.NumberOfQuestions;

                newExam.ExamQuestions = [.. question
                .Take(request.NumberOfQuestions)
                .Select(q => new ExamQuestions
                {
                    ExamId = newExam.Id,
                    QuestionId = q.Id,
                    QuestionMark = markPerQuestion,
                })];
            }
            else
            {
                foreach (var item in request.QuestionIdsWithMarks ?? Enumerable.Empty<KeyValuePair<Guid, decimal>>())
                {
                    newExam.ExamQuestions.Add(
                        new ExamQuestions
                        {
                            ExamId = newExam.Id,
                            QuestionId = item.Key,
                            QuestionMark = item.Value,
                        }
                    );
                }
            }
            await _mediator.Publish(new ExamAddedEvent(request.CourseId, request.SectionId), cancellationToken);

            await _unitOfWork.Repository<Exam>().AddAsync(newExam, cancellationToken);


            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<GenerateExamResponse>.Success(new GenerateExamResponse
            {
                Message = "Exam created successfully",
                ExamId = newExam.Id,
            });
        }
    }
}
