using Application.DTOs.Section;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Section.Query.GetSectionDetails
{
    public class GetSectionDetailsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetSectionDetailsQuery, Result<GetSectionDetailsResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<GetSectionDetailsResponse>> Handle(GetSectionDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _unitOfWork.GetRepository<ISectionRepository>()
                                                                     .GetSectionDetailsResponse(request.SectionId, cancellationToken);

                return Result<GetSectionDetailsResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<GetSectionDetailsResponse>.FailureStatusCode(
                    $"An error occurred while retrieving section details: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
