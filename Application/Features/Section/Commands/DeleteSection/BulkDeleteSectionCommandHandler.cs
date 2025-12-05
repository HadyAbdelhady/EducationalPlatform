using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Section.Commands.DeleteSection
{
    public class BulkDeleteSectionCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<BulkDeleteSectionCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<string>> Handle(BulkDeleteSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.SectionIds == null || request.SectionIds.Count == 0)
                    return Result<string>.FailureStatusCode("No section IDs provided", ErrorType.BadRequest);

                var sectionRepo = _unitOfWork.Repository<Domain.Entities.Section>();
                var courseRepo = _unitOfWork.Repository<Domain.Entities.Course>();

                var deletedCount = 0;

                foreach (var id in request.SectionIds)
                {
                    var section = await sectionRepo.GetByIdAsync(id, cancellationToken);

                    if (section == null) continue;
                    
                    var courseId = section.CourseId ?? Guid.Empty;
                    
                    var course = await courseRepo.GetByIdAsync(courseId, cancellationToken);

                    if (course != null)
                    {
                        course.NumberOfSections -= 1;
                        courseRepo.Update(course);
                    }

                    await sectionRepo.RemoveAsync(section.Id, cancellationToken);
                    deletedCount++;
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<string>.Success($"{deletedCount} sections deleted successfully");
            }
            catch (Exception ex)
            {
                return Result<string>.FailureStatusCode(
                    $"Error while bulk deleting sections: {ex.Message}",
                    ErrorType.InternalServerError
                );
            }
        }
    }
}
