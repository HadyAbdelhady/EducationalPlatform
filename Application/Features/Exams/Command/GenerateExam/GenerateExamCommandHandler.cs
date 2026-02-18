using Application.HelperFunctions;
using Application.DTOs.Questions;
using Application.ResultWrapper;
using Application.Interfaces;
using Domain.Entities;
using Domain.enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Exams.Command.GenerateExam
{
    public class GenerateExamCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<GenerateExamCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;


        public async Task<Result<string>> Handle(GenerateExamCommand request, CancellationToken cancellationToken)
        {
            var QuestionRepository = _unitOfWork.Repository<Question>();


            var question =  QuestionRepository.Find(q => q.CourseId == request.CourseId &&
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

            // Determine exam status based on start time
            ExamStatus examStatus = ExamStatus.Draft;
            if (request.ExamStartTime <= DateTimeOffset.UtcNow && request.ExamEndTime >= DateTimeOffset.UtcNow)
            {
                examStatus = ExamStatus.Started; // Exam is currently available
            }
            else if (request.ExamStartTime > DateTimeOffset.UtcNow)
            {
                examStatus = ExamStatus.Scheduled; // Exam is scheduled for the future
            }

            Exam newExam = new()
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
                PassMarkPercentage = request.PassMarkPercentage,
                InstructorId = request.CreatedBy,
                Status = examStatus,
            };

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
            await _mediator.Publish(new ExamAddedEvent(request.CourseId, request.SectionId), cancellationToken);

            await _unitOfWork.Repository<Exam>().AddAsync(newExam, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var studentExamResultRepository = _unitOfWork.Repository<StudentExamResult>();
            List<Guid> enrolledStudentIds = new();

            if (request.SectionId.HasValue)
            {
                // Get students enrolled in the section through Section navigation property
                var sectionRepository = _unitOfWork.Repository<Section>();
                var section = await sectionRepository
                    .FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken, s => s.StudentSections);
                
                if (section != null)
                {
                    enrolledStudentIds = section.StudentSections
                        .Select(ss => ss.StudentId)
                        .Distinct()
                        .ToList();
                }
            }
            else
            {
                // Get students enrolled in the course through Course navigation property
                var courseRepository = _unitOfWork.Repository<Course>();
                var course = await courseRepository
                    .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken, c => c.StudentCourses);
                
                if (course != null)
                {
                    enrolledStudentIds = course.StudentCourses
                        .Select(sc => sc.StudentId)
                        .Distinct()
                        .ToList();
                }
            }

            // Create StudentExamResult records for each enrolled student
            var studentExamResults = enrolledStudentIds.Select(studentId => new StudentExamResult
            {
                Id = Guid.NewGuid(),
                ExamId = newExam.Id,
                StudentId = studentId,
                Status = ExamResultStatus.NotStarted,
                CreatedAt = DateTimeOffset.UtcNow
            }).ToList();

            if (studentExamResults.Any())
            {
                await studentExamResultRepository.AddRangeAsync(studentExamResults, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

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
