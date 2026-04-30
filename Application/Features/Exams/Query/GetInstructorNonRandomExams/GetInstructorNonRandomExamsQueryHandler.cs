using Application.DTOs.Exam;
using Application.HelperFunctions;
using Application.Interfaces;
using Application.Interfaces.BaseFilters;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Exams.Query.GetInstructorNonRandomExams
{
    public class GetInstructorNonRandomExamsQueryHandler(
        IUnitOfWork unitOfWork,
        IBaseFilterRegistry<Exam> examFilterRegistry) : IRequestHandler<GetInstructorNonRandomExamsQuery, Result<InstructorNonRandomExamsResult>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<Exam> _examFilterRegistry = examFilterRegistry;
        private readonly IExamRepository _examRepository = unitOfWork.Repository<Exam>() as IExamRepository;

        public async Task<Result<InstructorNonRandomExamsResult>> Handle(
            GetInstructorNonRandomExamsQuery request,
            CancellationToken cancellationToken)
        {
            // Get non-randomized exams for the instructor
            var examsQuery = _unitOfWork.Repository<Exam>()
                .GetAll(cancellationToken)
                .Where(e => e.InstructorId == request.Request.InstructorId && !e.IsRandomized)
                .ApplyFilters(request.Request.RequestSkeleton.Filters, _examFilterRegistry.Filters)
                .ApplySort(request.Request.RequestSkeleton.SortBy, request.Request.RequestSkeleton.IsDescending, _examFilterRegistry.Sorts);

            var exams = await examsQuery
                .Include(e => e.Course)
                .Include(e => e.Section)
                .Include(e => e.ExamResults)
                .ToListAsync(cancellationToken);

            var examDtos = exams.Select(e => new InstructorNonRandomExamsResponseDto
            {
                ExamId = e.Id,
                Name = e.Name,
                Description = e.Description,
                ExamStatus = e.Status,
                ExamType = e.ExamType,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                TotalMark = e.TotalMark,
                NumberOfQuestions = e.NumberOfQuestions,
                DurationInMinutes = e.DurationInMinutes,
                IsRandomized = e.IsRandomized,
                PassMarkPercentage = e.PassMarkPercentage,
                CourseId = e.CourseId,
                CourseName = e.Course?.Name ?? string.Empty,
                SectionId = e.SectionId,
                SectionName = e.Section?.Name,
                
                // Calculate exam statistics
                StudentCount = e.ExamResults.Count,
                PassedCount = e.ExamResults.Count(r => r.Status == ExamResultStatus.Passed),
                FailedCount = e.ExamResults.Count(r => r.Status == ExamResultStatus.Failed),
                NotStartedCount = e.ExamResults.Count(r => r.Status == ExamResultStatus.NotStarted),
                InProgressCount = e.ExamResults.Count(r => r.Status == ExamResultStatus.InProgress)
            }).ToList();

            // Pagination
            int pageSize = 10;
            int skip = (request.Request.RequestSkeleton.PageNumber - 1) * pageSize;
            var paginatedExams = examDtos.Skip(skip).Take(pageSize).ToList();

            // Get courses and sections hashmap from repository
            var coursesSectionsHashMap = await _examRepository.GetInstructorCoursesSectionsHashMapAsync(request.Request.InstructorId, cancellationToken);

            // Convert to the expected DTO structure
            var coursesSectionsDto = new CoursesSectionsHashMap();
            foreach (var courseEntry in coursesSectionsHashMap)
            {
                var course = await _unitOfWork.Repository<Course>().GetByIdAsync(courseEntry.Key, cancellationToken);
                if (course != null)
                {
                    coursesSectionsDto.Courses[courseEntry.Key] = new CourseSectionInfo
                    {
                        Id = course.Id,
                        Name = course.Name,
                        Sections = new Dictionary<Guid, SectionInfo>()
                    };

                    foreach (var sectionEntry in courseEntry.Value)
                    {
                        coursesSectionsDto.Courses[courseEntry.Key].Sections[sectionEntry.Key] = new SectionInfo
                        {
                            Id = sectionEntry.Key,
                            Name = sectionEntry.Value
                        };
                    }
                }
            }

            return Result<InstructorNonRandomExamsResult>.Success(
                new InstructorNonRandomExamsResult
                {
                    Exams = new PaginatedResult<InstructorNonRandomExamsResponseDto>
                    {
                        Items = paginatedExams,
                        PageNumber = request.Request.RequestSkeleton.PageNumber,
                        PageSize = pageSize,
                        TotalCount = examDtos.Count
                    },
                    CoursesSections = coursesSectionsDto
                });
        }
    }
}
