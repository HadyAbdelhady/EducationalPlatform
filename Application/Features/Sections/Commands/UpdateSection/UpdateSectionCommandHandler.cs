using Application.Features.Sections.DTOs;
using Application.Common.Interfaces;
using Application.Common;
using Domain.Entities;
using Domain.enums;
using MediatR;
namespace Application.Features.Sections.Commands.UpdateSection
{
    public class UpdateSectionCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateSectionCommand, Result<SectionUpdateResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<SectionUpdateResponse>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sectionRepo = _unitOfWork.Repository<Section>();

                var section = await sectionRepo.FirstOrDefaultAsync(s => s.Id == request.SectionId, cancellationToken);
                if (section == null)
                    return Result<SectionUpdateResponse>.FailureStatusCode("Section not found.", ErrorType.NotFound);
                if (!string.IsNullOrWhiteSpace(request.Name))
                    section.Name = request.Name;
                if (!string.IsNullOrWhiteSpace(request.Description))
                    section.Description = request.Description;

                if (request.Price.HasValue && section.StudentSections.Count == 0)
                    section.Price = request.Price.Value;

                if (request.CourseId != section.CourseId)
                {
                    var hasEnrolledStudentsInSection = await sectionRepo
                        .AnyAsync(s => s.Id == request.SectionId && s.StudentSections.Any(), cancellationToken);

                    var hasEnrolledStudentsInCourse = await _unitOfWork.Repository<Course>()
                        .AnyAsync(c => c.Id == section.CourseId && c.StudentCourses.Any(), cancellationToken);

                    if (hasEnrolledStudentsInSection || hasEnrolledStudentsInCourse)
                    {
                        return Result<SectionUpdateResponse>.FailureStatusCode(
                            "Cannot move this section because there are students enrolled.",
                            ErrorType.Conflict);
                    }

                    section.CourseId = request.CourseId;
                }

                section.UpdatedAt = EgyptTime.UtcNow;

                sectionRepo.Update(section);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<SectionUpdateResponse>.Success(new SectionUpdateResponse
                {
                    SectionId = section.Id,
                    Name = section.Name,
                    UpdatedAt = section.UpdatedAt?.UtcDateTime ?? EgyptTime.UtcNow.DateTime
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<SectionUpdateResponse>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<SectionUpdateResponse>.FailureStatusCode($"Error updating section: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
