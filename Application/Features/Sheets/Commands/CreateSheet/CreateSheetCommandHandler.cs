using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Sheets.Commands.CreateSheet
{
    public class CreateSheetCommandHandler(IUnitOfWork unitOfWork, ICloudinaryCore cloudinaryService) : IRequestHandler<CreateSheetCommand, Result<SheetCreationResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<SheetCreationResponse>> Handle(CreateSheetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var instructorExists = await _unitOfWork.Repository<User>().AnyAsync(i => i.Id == request.InstructorId, cancellationToken);

                if (!instructorExists)
                {
                    return Result<SheetCreationResponse>.FailureStatusCode("Instructor not found", ErrorType.NotFound);
                }

                if (request.SectionId is not null)
                {
                    var sectionExists = await _unitOfWork.Repository<Section>().AnyAsync(s => s.Id == request.SectionId, cancellationToken);

                    if (!sectionExists)
                    {
                        return Result<SheetCreationResponse>.FailureStatusCode("Section not found", ErrorType.NotFound);
                    }
                }
                else if (request.VideoId is not null)
                {
                    var videoExists = await _unitOfWork.Repository<Video>().AnyAsync(v => v.Id == request.VideoId, cancellationToken);

                    if (!videoExists)
                    {
                        return Result<SheetCreationResponse>.FailureStatusCode("Video not found", ErrorType.NotFound);
                    }
                }
                else if (request.CourseId is not null)
                {
                    var courseExists = await _unitOfWork.Repository<Course>().AnyAsync(c => c.Id == request.CourseId, cancellationToken);

                    if (!courseExists)
                    {
                        return Result<SheetCreationResponse>.FailureStatusCode("Course not found", ErrorType.NotFound);
                    }

                }

                var cloudianryResult = await _cloudinaryService.UploadPdfAsync(request.SheetUrl);
                var newSheet = new Sheet()
                {
                    Id = new Guid(),
                    Name = request.Name,
                    Type = request.Type,
                    DueDate = request.DueDate,
                    SheetUrl = cloudianryResult.Url,
                    SheetPublicId = cloudianryResult.PublicId,
                    InstructorId = request.InstructorId,
                    SectionId = request.SectionId,
                    CourseId = request.CourseId,
                    VideoId = request.VideoId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    IsDeleted = false
                };

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
