using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Section.Query.GetAllSectionsByCourse
{
    public class GetAllSectionsByCourseQueryHandler : IRequestHandler<GetAllSectionsByCourseQuery, Result<List<SectionResponse>>>
    {
        public Task<Result<List<SectionResponse>>> Handle(GetAllSectionsByCourseQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class SectionResponse
    {
    }
}
