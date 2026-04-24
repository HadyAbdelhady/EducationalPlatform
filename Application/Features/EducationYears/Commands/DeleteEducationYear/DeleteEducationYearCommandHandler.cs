using Application.Interfaces;
using Application.ResultWrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.EducationYears.Commands.DeleteEducationYear
{
    public class DeleteEducationYearCommandHandler(IUnitOfWork unitOfWork) 
        : IRequestHandler<DeleteEducationYearCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<bool>> Handle(DeleteEducationYearCommand request, CancellationToken cancellationToken)
        {
            var educationYear = await _unitOfWork.EducationYears
                .FirstOrDefaultAsync(ey => ey.Id == request.Id && !ey.IsDeleted, cancellationToken);

            if (educationYear == null)
            {
                return Result<bool>.Failure(
                    "Education year not found.",
                    ResultType.NotFound
                );
            }

            // Check if education year has associated students or courses
            var hasStudents = await _unitOfWork.Students
                .AnyAsync(s => s.EducationYearId == request.Id && !s.IsDeleted, cancellationToken);

            var hasCourses = await _unitOfWork.Courses
                .AnyAsync(c => c.EducationYearId == request.Id && !c.IsDeleted, cancellationToken);

            if (hasStudents || hasCourses)
            {
                return Result<bool>.Failure(
                    "Cannot delete education year. It has associated students or courses.",
                    ResultType.Conflict
                );
            }

            educationYear.IsDeleted = true;
            educationYear.UpdatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
