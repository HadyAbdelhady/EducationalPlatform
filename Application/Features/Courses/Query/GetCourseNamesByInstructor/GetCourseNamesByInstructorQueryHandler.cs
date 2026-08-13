using Application.Common.Interfaces;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Courses.Query.GetCourseNamesByInstructor
{
    public class GetCourseNamesByInstructorQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCourseNamesByInstructorQuery, Result<List<CourseData>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<List<CourseData>>> Handle(GetCourseNamesByInstructorQuery request, CancellationToken cancellationToken)
        {
            var courses = await _unitOfWork.Repository<Course>().GetAll(cancellationToken)
                                                    .Where(c => c.InstructorCourses
                                                    .Any(ic => ic.InstructorId == request.InstructorId && c.EducationYearId == request.EducationalYearId))
                .Select(c => new CourseData { Id = c.Id, Name = c.Name })
                .ToListAsync(cancellationToken);

            return Result<List<CourseData>>.Success(courses);

        }
    }
}
