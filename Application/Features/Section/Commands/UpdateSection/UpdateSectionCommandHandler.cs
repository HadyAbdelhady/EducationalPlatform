using Application.DTOs.Section;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;
namespace Application.Features.Section.Commands.UpdateSection
{
    public class UpdateSectionCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateSectionCommand, Result<SectionUpdateResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<SectionUpdateResponse>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var sectionRepo = _unitOfWork.Repository<Domain.Entities.Section>();

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
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<SectionUpdateResponse>.Success(new SectionUpdateResponse
                {
                    SectionId = section.Id,
                    Name = section.Name,
                    UpdatedAt = section.UpdatedAt.UtcDateTime
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<SectionUpdateResponse>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<SectionUpdateResponse>.FailureStatusCode($"Error updating section: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
