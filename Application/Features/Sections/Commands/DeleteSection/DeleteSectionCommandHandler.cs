using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Sections.Commands.DeleteSection
{
    public class DeleteSectionCommandHandler(IUnitOfWork unitOfWork): IRequestHandler<DeleteSectionCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<string>> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sectionRepo = _unitOfWork.Repository<Section>();
                var courseRepo = _unitOfWork.Repository<Course>();

                var section = await sectionRepo.GetByIdAsync(request.SectionId, cancellationToken)
                    ?? throw new KeyNotFoundException("Section not found");

                if (section.CourseId == null)
                    return Result<string>.FailureStatusCode("Section has no assigned course.", ErrorType.BadRequest);

                var course = await courseRepo.GetByIdAsync(section.CourseId.Value, cancellationToken)
                    ?? throw new KeyNotFoundException("Course not found for this section");

                if (course.NumberOfSections > 0)
                    course.NumberOfSections -= 1;

                courseRepo.Update(course);

                await sectionRepo.RemoveAsync(section.Id, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<string>.Success("Section deleted successfully");
            }
            catch (KeyNotFoundException knfEx)
            {
                return Result<string>.FailureStatusCode(knfEx.Message, ErrorType.NotFound);
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<string>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<string>.FailureStatusCode(
                    $"An error occurred while deleting the section: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
