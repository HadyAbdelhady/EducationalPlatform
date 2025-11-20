using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Section.Commands.DeleteSection
{
    public class DeleteSectionCommandHandler(IUnitOfWork unitOfWork): IRequestHandler<DeleteSectionCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<string>> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var sectionRepo = _unitOfWork.Repository<Domain.Entities.Section>();
                var courseRepo = _unitOfWork.Repository<Domain.Entities.Course>();

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
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<string>.Success("Section deleted successfully");
            }
            catch (KeyNotFoundException knfEx)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<string>.FailureStatusCode(knfEx.Message, ErrorType.NotFound);
            }
            catch (UnauthorizedAccessException auth)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<string>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<string>.FailureStatusCode(
                    $"An error occurred while deleting the section: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
