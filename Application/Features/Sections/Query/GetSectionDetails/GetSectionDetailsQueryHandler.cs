using Application.DTOs.Sections;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Sections.Query.GetSectionDetails
{
    public class GetSectionDetailsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetSectionDetailsQuery, Result<SectionDetailsQueryModel>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<SectionDetailsQueryModel>> Handle(GetSectionDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _unitOfWork.GetRepository<ISectionRepository>()
                                                                     .GetSectionDetailsResponse(request, cancellationToken);

                return Result<SectionDetailsQueryModel>.Success(data);
            }
            catch (Exception ex)
            {
                return Result<SectionDetailsQueryModel>.FailureStatusCode(
                    $"An error occurred while retrieving section details: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
