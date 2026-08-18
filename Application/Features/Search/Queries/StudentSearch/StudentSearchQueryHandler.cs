using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Centers.Interfaces;
using Application.Features.EducationYears.Interfaces;
using Application.Features.Search.DTOs;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Search.Queries.StudentSearch
{
    public class StudentSearchQueryHandler(
        IUnitOfWork unitOfWork,
        IStudentEducationYearProvider studentEducationYearProvider,
        ICenterRepository centerRepository)
        : IRequestHandler<StudentSearchQuery, Result<StudentSearchResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IStudentEducationYearProvider _studentEducationYearProvider = studentEducationYearProvider;
        private readonly ICenterRepository _centerRepository = centerRepository;

        public async Task<Result<StudentSearchResponse>> Handle(
            StudentSearchQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var term = request.Query?.Trim() ?? string.Empty;
                if (term.Length < 2)
                {
                    return Result<StudentSearchResponse>.FailureStatusCode(
                        "Search term must be at least 2 characters.",
                        ErrorType.BadRequest);
                }

                var page = request.Page < 1 ? 1 : request.Page;
                var pageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 25);

                var educationYearId = await _studentEducationYearProvider
                    .GetEducationYearIdByUserIdAsync(request.UserId, cancellationToken);

                if (!educationYearId.HasValue)
                {
                    return Result<StudentSearchResponse>.Success(new StudentSearchResponse
                    {
                        Courses = QueryableFilterExtensions.EmptyPaginatedResult<CourseSearchItemDto>(page, pageSize),
                        Sections = QueryableFilterExtensions.EmptyPaginatedResult<SectionSearchItemDto>(page, pageSize),
                        Instructors = null
                    });
                }

                var studentId = request.UserId;

                var coursesQuery = _unitOfWork.Repository<Course>()
                    .GetAll(cancellationToken)
                    .AsNoTracking()
                    .Where(c => c.EducationYearId == educationYearId.Value && c.Name.Contains(term))
                    .OrderBy(c => c.Name)
                    .Select(c => new CourseSearchItemDto
                    {
                        Id = c.Id,
                        Title = c.Name,
                        PictureUrl = c.PictureUrl,
                        Price = c.Price ?? 0,
                        IsEnrolled = c.StudentCourses.Any(sc => sc.StudentId == studentId)
                    });

                var sectionsQuery = _unitOfWork.Repository<Section>()
                    .GetAll(cancellationToken)
                    .AsNoTracking()
                    .Where(s => s.Course!.EducationYearId == educationYearId.Value && s.Name.Contains(term))
                    .OrderBy(s => s.Name)
                    .Select(s => new SectionSearchItemDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        CourseId = s.CourseId,
                        CourseName = s.Course!.Name,
                        IsEnrolled = s.StudentSections.Any(ss => ss.StudentId == studentId)
                            || s.Course!.StudentCourses.Any(sc => sc.StudentId == studentId)
                    });

                var courses = await coursesQuery.ToPaginatedResultAsync(page, pageSize, cancellationToken);
                var sections = await sectionsQuery.ToPaginatedResultAsync(page, pageSize, cancellationToken);

                PaginatedResult<InstructorSearchItemDto>? instructors = null;
                var centerId = await _centerRepository.GetStudentCenterIdAsync(request.UserId, cancellationToken);
                if (centerId.HasValue)
                {
                    var instructorsQuery = _unitOfWork.Repository<Center>()
                        .GetAll(cancellationToken)
                        .AsNoTracking()
                        .Where(c => c.Id == centerId.Value)
                        .SelectMany(c => c.CenterInstructors)
                        .Where(ci => !ci.IsDeleted && ci.Instructor.User.FullName.Contains(term))
                        .OrderBy(ci => ci.Instructor.User.FullName)
                        .Select(ci => new InstructorSearchItemDto
                        {
                            Id = ci.InstructorId,
                            FullName = ci.Instructor.User.FullName,
                            PersonalPictureUrl = ci.Instructor.User.PersonalPictureUrl
                        });

                    instructors = await instructorsQuery.ToPaginatedResultAsync(page, pageSize, cancellationToken);
                }

                return Result<StudentSearchResponse>.Success(new StudentSearchResponse
                {
                    Courses = courses,
                    Sections = sections,
                    Instructors = instructors
                });
            }
            catch (Exception ex)
            {
                return Result<StudentSearchResponse>.FailureStatusCode(
                    $"An error occurred while searching: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
