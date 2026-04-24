using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.EducationYears.Commands.DeleteEducationYear
{
    public class DeleteEducationYearCommandHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<DeleteEducationYearCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<bool>> Handle(DeleteEducationYearCommand request, CancellationToken cancellationToken)
        {
            var educationYear = await _unitOfWork.Repository<EducationYear>()
                .FirstOrDefaultAsync(ey => ey.Id == request.Id && !ey.IsDeleted, cancellationToken);

            if (educationYear == null)
            {
                return Result<bool>.FailureStatusCode(
                    "Education year not found.",
                    ErrorType.NotFound
                );
            }

            // Check if education year has associated students or courses
            var hasStudents = await _unitOfWork.Repository<User>()
                .AnyAsync(s => s.Student!.EducationYearId == request.Id, cancellationToken);

            var hasCourses = await _unitOfWork.Repository<Course>()
                .AnyAsync(c => c.EducationYearId == request.Id, cancellationToken);

            if (hasStudents || hasCourses)
            {
                return Result<bool>.FailureStatusCode(
                    "Cannot delete education year. It has associated students or courses.",
                    ErrorType.Conflict
                );
            }

            educationYear.IsDeleted = true;
            educationYear.UpdatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
