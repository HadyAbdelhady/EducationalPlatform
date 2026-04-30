using Application.DTOs;
using Application.DTOs.Sections;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sections.Query.GetSectionsForCourse
{
    public record GetSectionsForCourseQuery : IRequest<Result<List<SectionDetailsQueryModel>>>
    {
        public Guid CourseId { get; init; }
        public Guid UserId { get; init; }
        public GetAllEntityRequestSkeleton GetAllEntityRequestSkeleton { get; init; } = new GetAllEntityRequestSkeleton();
    }

   

}
