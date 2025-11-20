using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Section.Query.GetSectionByID
{
    public class GetSectionByIDQueryHandler : IRequestHandler<GetSectionByIDQuery, Result<GetSectionByIDResponse>>
    {
        public Task<Result<GetSectionByIDResponse>> Handle(GetSectionByIDQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class GetSectionByIDResponse
    {
    }
}
