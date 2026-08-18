using Application.Features.Exams.DTOs;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Exams.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Exams.Query.GetInstructorExams
{
    public class GetInstructorExamsQueryHandler(
        IUnitOfWork unitOfWork,
        IBaseFilterRegistry<InstructorExamsResponseDto> instructorExamsFilterRegistry) : IRequestHandler<GetInstructorExamsQuery, Result<InstructorExamsResult>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<InstructorExamsResponseDto> _instructorExamsFilterRegistry = instructorExamsFilterRegistry;
        private readonly IExamRepository _examRepository = unitOfWork.GetRepository<IExamRepository>();

        public async Task<Result<InstructorExamsResult>> Handle(
            GetInstructorExamsQuery request,
            CancellationToken cancellationToken)
        {
            var examsQuery = _examRepository.GetInstructorNonRandomExamsQuery(request.Request.InstructorId);

            var filteredSortedQuery = examsQuery
                .ApplyFilters(request.Request.RequestSkeleton.Filters, _instructorExamsFilterRegistry.Filters)
                .ApplySort(request.Request.RequestSkeleton.SortBy, request.Request.RequestSkeleton.IsDescending, _instructorExamsFilterRegistry.Sorts);

            var paginatedExams = await filteredSortedQuery.ToPaginatedResultAsync(
                request.Request.RequestSkeleton.PageNumber,
                10,
                cancellationToken);

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

            return Result<InstructorExamsResult>.Success(
                new InstructorExamsResult
                {
                    Exams = paginatedExams,
                    CoursesSections = coursesSectionsDto
                });
        }
    }
}
