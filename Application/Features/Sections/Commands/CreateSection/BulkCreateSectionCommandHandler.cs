using Application.DTOs.Sections;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Sections.Commands.CreateSection
{
    public class BulkCreateSectionCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        : IRequestHandler<BulkCreateSectionCommand, Result<List<CreateSectionResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;

        public async Task<Result<List<CreateSectionResponse>>> Handle(BulkCreateSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sectionRepo = _unitOfWork.Repository<Section>();
                var courseRepo = _unitOfWork.Repository<Course>();

                // ensure the target course exists to avoid FK violations
                var course = await courseRepo.GetByIdAsync(request.CourseId, cancellationToken);
                if (course == null)
                {
                    return Result<List<CreateSectionResponse>>.FailureStatusCode("Course not found.", ErrorType.NotFound);
                }

                var responses = new List<CreateSectionResponse>();

                foreach (var section in request.Sections)
                {
                    var newSection = new Section
                    {
                        Id = Guid.NewGuid(),
                        Name = section.Name,
                        Description = section.Description,
                        Price = section.Price,
                        CourseId = request.CourseId,
                        CreatedAt = DateTimeOffset.UtcNow
                    };

                    await sectionRepo.AddAsync(newSection, cancellationToken);
                    responses.Add(new CreateSectionResponse
                    {
                        SectionId = newSection.Id,
                        Name = newSection.Name,
                        CreatedAt = newSection.CreatedAt.UtcDateTime
                    });
                }

                // persist changes before publishing events that may rely on the data
                await _mediator.Publish(new SectionAddedEvent(request.CourseId, responses.Count), cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<List<CreateSectionResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                return Result<List<CreateSectionResponse>>
                    .FailureStatusCode($"Error in bulk create: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
