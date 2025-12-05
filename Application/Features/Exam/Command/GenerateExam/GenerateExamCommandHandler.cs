using Application.DTOs.Question;
using Application.HelperFunctions;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Exam.Command.GenerateExam
{
    public class GenerateExamCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<GenerateExamCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<string>> Handle(GenerateExamCommand request, CancellationToken cancellationToken)
        {
            var QuestionRepository = _unitOfWork.Repository<Domain.Entities.Question>();


            var question = await QuestionRepository.FindAsync(q => q.CourseId == request.CourseId &&
                                                                                        (!request.SectionId.HasValue || q.SectionId == request.SectionId)
                                                                                        , cancellationToken);

            if (!question.Any())
            {
                return Result<string>.FailureStatusCode("Question does not exist.", ErrorType.NotFound);
            }

            if (question.Count() < request.NumberOfQuestions)
            {
                return Result<string>.FailureStatusCode(
                    $"Not enough questions available. Requested: {request.NumberOfQuestions}, Available: {question.Count()}.",
                    ErrorType.BadRequest
                );
            }

            Domain.Entities.Exam newExam = new()
            {
                Id = Guid.NewGuid(),
                CourseId = request.CourseId,
                SectionId = request.SectionId,
                Name = request.Title,
                TotalMark = request.ExamTotalMark,
                Description = request.Description,
                NumberOfQuestions = request.NumberOfQuestions,
                EndTime = request.ExamEndTime,
                StartTime = request.ExamStartTime,
                ExamType = request.ExamType,
                IsRandomized = request.IsRandomized,
                DurationInMinutes = request.DurationInMinutes,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            newExam.InstructorExams.Add(new InstructorExam
            {
                ExamId = newExam.Id,
                InstructorId = request.CreatedBy,
            });

            if (request.IsRandomized)
            {
                question.ToList().Shuffle();
                decimal markPerQuestion = request.ExamTotalMark / request.NumberOfQuestions;

                newExam.ExamQuestions = [.. question
                .Take(request.NumberOfQuestions)
                .Select(q => new ExamBank
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
                        new ExamBank
                        {
                            ExamId = newExam.Id,
                            QuestionId = item.Key,
                            QuestionMark = item.Value,
                        }
                    );
                }
            }

            await _unitOfWork.Repository<Domain.Entities.Exam>().AddAsync(newExam, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new AddQuestionToExamBankResponse
            {
                CourseId = newExam.CourseId,
                SectionId = newExam.SectionId,
                ExamId = newExam.Id
            };
            return Result<string>.Success($"Exam created successfully with ID: {newExam.Id} for Course ID: {newExam.CourseId} and Section ID: {newExam.SectionId}");
        }
    }
}
