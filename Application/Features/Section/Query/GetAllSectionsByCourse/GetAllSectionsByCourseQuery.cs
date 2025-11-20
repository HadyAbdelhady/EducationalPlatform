using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Section.Query.GetAllSectionsByCourse
{
    public class GetAllSectionsByCourseQuery : IRequest<Result<List<SectionResponse>>>
    {
    }
}
