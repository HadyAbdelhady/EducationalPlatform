using Application.DTOs.Section;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Section.Commands.CreateSection
{
    public class CreateSectionCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<CreateSectionCommand, Result<CreateSectionResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<CreateSectionResponse>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

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

                var courseRepo = _unitOfWork.Repository<Domain.Entities.Course>();
                var course = await courseRepo.GetByIdAsync(request.CourseId, cancellationToken);
                if (course != null)
                {
                    course.NumberOfSections += 1;
                    course.UpdatedAt = DateTimeOffset.UtcNow;
                    courseRepo.Update(course);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

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
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                return Result<CreateSectionResponse>
                    .FailureStatusCode($"Error creating section: {ex.Message}", ErrorType.NotCreated);
            }
        }
    }
}
