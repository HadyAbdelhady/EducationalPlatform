using Application.DTOs.Sections;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sections.Query.GetSectionsForCourse
{
    public record GetSectionsForCourseQuery(Guid CourseId, Guid UserId) : IRequest<Result<List<GetSectionResponse>>>;

}
