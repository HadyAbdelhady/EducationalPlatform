using Application.DTOs.Section;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Section.Commands.CreateSection
{
    public class BulkCreateSectionCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<BulkCreateSectionCommand, Result<List<CreateSectionResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<List<CreateSectionResponse>>> Handle(BulkCreateSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sectionRepo = _unitOfWork.Repository<Domain.Entities.Section>();
                var courseRepo = _unitOfWork.Repository<Domain.Entities.Course>();

                var responses = new List<CreateSectionResponse>();

                foreach (var section in request.Sections)
                {
                    var newSection = new Domain.Entities.Section
                    {
                        Id = Guid.NewGuid(),
                        Name = section.Name,
                        Description = section.Description,
                        Price = section.Price,
                        CourseId = section.CourseId,
                        CreatedAt = DateTimeOffset.UtcNow,
                        UpdatedAt = DateTimeOffset.UtcNow
                    };

                    await sectionRepo.AddAsync(newSection, cancellationToken);
                    responses.Add(new CreateSectionResponse
                    {
                        SectionId = newSection.Id,
                        Name = newSection.Name,
                        CreatedAt = newSection.CreatedAt.UtcDateTime
                    });

                    var course = await courseRepo.GetByIdAsync(section.CourseId, cancellationToken);
                    if (course != null)
                    {
                        course.NumberOfSections += 1;
                        course.UpdatedAt = DateTimeOffset.UtcNow;
                        courseRepo.Update(course);
                    }
                }

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
