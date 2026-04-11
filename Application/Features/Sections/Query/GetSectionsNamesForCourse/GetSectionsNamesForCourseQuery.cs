using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sections.Query.GetSectionsNamesFourCourse
{
    public class GetSectionsNamesForCourseQuery : IRequest<Result<List<SectionData>>>
    {
        public Guid CourseId { get; set; }
    }

    public class SectionData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
