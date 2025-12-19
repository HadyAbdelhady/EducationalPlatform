using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sections.Query.GetAllSectionsByCourse
{
    public class GetAllSectionsByCourseQuery : IRequest<Result<List<SectionResponse>>>
    {
    }
}
