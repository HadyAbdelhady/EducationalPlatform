using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Sheets.Commands.CreateSheet
{
    public class CreateSheetCommandHandler(IUnitOfWork unitOfWork, ICloudinaryCore cloudinaryService, IMediator mediator) : IRequestHandler<CreateSheetCommand, Result<SheetCreationResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;
        private readonly IMediator _mediator = mediator;

        public async Task<Result<SheetCreationResponse>> Handle(CreateSheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var instructor = await _unitOfWork.GetRepository<IUserRepository>().DoesInstructorExistAsync(request.InstructorId, cancellationToken);
                EntityType EntityType = EntityType.None;
                Guid EventEntity = Guid.Empty;

                if (!instructor)
                {
                    return Result<SheetCreationResponse>.FailureStatusCode("Instructor not found", ErrorType.NotFound);
                }

                if (request.SectionId is not null)
                {
                    var sectionExists = await _unitOfWork.Repository<Section>().AnyAsync(s => s.Id == request.SectionId, cancellationToken);
                    EntityType = EntityType.Section;
                    if (!sectionExists)
                    {
                        return Result<SheetCreationResponse>.FailureStatusCode("Section not found", ErrorType.NotFound);
                    }
                    EventEntity = (Guid)request.SectionId!;
                }
                else if (request.VideoId is not null)
                {
                    var videoExists = await _unitOfWork.Repository<Video>().AnyAsync(v => v.Id == request.VideoId, cancellationToken);
                    if (!videoExists)
                    {
                        return Result<SheetCreationResponse>.FailureStatusCode("Video not found", ErrorType.NotFound);
                    }
                    EventEntity = (Guid)request.VideoId!;

                }
                else if (request.CourseId is not null)
                {
                    var courseExists = await _unitOfWork.Repository<Course>().AnyAsync(c => c.Id == request.CourseId, cancellationToken);
                    EntityType = EntityType.Course;
                    if (!courseExists)
                    {
                        return Result<SheetCreationResponse>.FailureStatusCode("Course not found", ErrorType.NotFound);
                    }
                    EventEntity = (Guid)request.CourseId!;
                }

                var cloudianryResult = await _cloudinaryService.UploadPdfAsync(request.SheetUrl);
                
                // Convert DueDate to UTC if provided (PostgreSQL requires UTC for timestamp with time zone)
                var dueDateUtc = request.DueDate?.ToUniversalTime();
                
                var newSheet = new Sheet()
                {
                    Id = new Guid(),
                    Name = request.Name,
                    Type = request.Type,
                    DueDate = dueDateUtc,
                    SheetUrl = cloudianryResult.Url,
                    SheetPublicId = cloudianryResult.PublicId,
                    InstructorId = request.InstructorId,
                    SectionId = request.SectionId,
                    CourseId = request.CourseId,
                    VideoId = request.VideoId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsDeleted = false
                };

                if (request.Type == SheetType.QuestionSheet)
                    await _mediator.Publish(new QuestionSheetAddedEvent(EventEntity, EntityType), cancellationToken);


                await _unitOfWork.Repository<Sheet>().AddAsync(newSheet, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<SheetCreationResponse>.Success(new SheetCreationResponse
                {
                    SheetId = newSheet.Id,
                    SheetUrl = newSheet.SheetUrl,
                    CreatedAt = newSheet.CreatedAt
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<SheetCreationResponse>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<SheetCreationResponse>.FailureStatusCode($"Error creating sheet: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
