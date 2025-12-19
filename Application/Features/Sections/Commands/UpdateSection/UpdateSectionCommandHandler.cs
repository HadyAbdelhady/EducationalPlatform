using Application.DTOs.Sections;
using Application.Interfaces;
using Application.ResultWrapper;
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

                section.Name = request.Name;
                section.Description = request.Description;
                section.Price = request.Price;
                section.CourseId = request.CourseId;
                section.UpdatedAt = DateTimeOffset.UtcNow;

                sectionRepo.Update(section);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<SectionUpdateResponse>.Success(new SectionUpdateResponse
                {
                    SectionId = section.Id,
                    Name = section.Name,
                    UpdatedAt = section.UpdatedAt?.UtcDateTime ?? DateTime.UtcNow
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
