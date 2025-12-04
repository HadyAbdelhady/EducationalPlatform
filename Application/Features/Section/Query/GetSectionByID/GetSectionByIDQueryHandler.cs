using Application.DTOs.Section;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Section.Query.GetSectionByID
{
    public class GetSectionByIDQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetSectionByIDQuery, Result<GetSectionByIDResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<GetSectionByIDResponse>> Handle(GetSectionByIDQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var section = await _unitOfWork.Repository<Domain.Entities.Section>()
                    .GetByIdAsync(request.SectionId, cancellationToken);

                if (section == null || section.IsDeleted)
                {
                    return Result<GetSectionByIDResponse>.FailureStatusCode(
                        $"Section with ID {request.SectionId} not found.",
                        ErrorType.NotFound);
                }

                var response = new GetSectionByIDResponse
                {
                    SectionId = section.Id,
                    Name = section.Name,
                    Description = section.Description,
                    Price = section.Price,
                    NumberOfVideos = section.NumberOfVideos,
                    Rating = section.Rating,
                    CourseId = section.CourseId ?? Guid.Empty,
                    CreatedAt = section.CreatedAt,
                    UpdatedAt = section.UpdatedAt
                };

                return Result<GetSectionByIDResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<GetSectionByIDResponse>.FailureStatusCode(
                    $"An error occurred while retrieving the section: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
