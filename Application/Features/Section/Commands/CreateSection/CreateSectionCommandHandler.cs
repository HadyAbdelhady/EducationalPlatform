using Application.DTOs.Section;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Section.Commands.CreateSection
{
    public class CreateSectionCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        : IRequestHandler<CreateSectionCommand, Result<CreateSectionResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;

        public async Task<Result<CreateSectionResponse>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var newSection = new Domain.Entities.Section
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    NumberOfVideos = request.NumberOfVideos,
                    CourseId = request.CourseId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                };

                await _unitOfWork.Repository<Domain.Entities.Section>()
                    .AddAsync(newSection, cancellationToken);

                await _mediator.Publish(new SectionAddedEvent(newSection.Id, request.CourseId), cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                
                return Result<CreateSectionResponse>.Success(new CreateSectionResponse
                {
                    SectionId = newSection.Id,
                    Name = newSection.Name,
                    CreatedAt = newSection.CreatedAt.UtcDateTime
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<CreateSectionResponse>
                    .FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<CreateSectionResponse>
                    .FailureStatusCode($"Error creating section: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
