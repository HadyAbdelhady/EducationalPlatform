using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using Domain.Events;
using MediatR;

namespace Application.Features.Section.Commands.DeleteSection
{
    public class BulkDeleteSectionCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        : IRequestHandler<BulkDeleteSectionCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;

        public async Task<Result<string>> Handle(BulkDeleteSectionCommand request, CancellationToken cancellationToken)
        {
            if (request.SectionIds == null || request.SectionIds.Count == 0)
                return Result<string>.FailureStatusCode("No section IDs provided", ErrorType.BadRequest);

            var course = await _unitOfWork.Repository<Domain.Entities.Course>()
                .GetByIdAsync(request.CourseId, cancellationToken);

            if (course == null)
                return Result<string>.FailureStatusCode("Course not found", ErrorType.NotFound);

            var sectionRepo = _unitOfWork.Repository<Domain.Entities.Section>();

            // Optional: Validate that all sections belong to this course (defensive)
            var sections = await sectionRepo
                                .FindAsync(s => request.SectionIds.Contains(s.Id) && s.CourseId == request.CourseId, cancellationToken);

            var SectionList = sections.ToList();

            /*
             Warn or fail if some IDs don't exist or don't belong to the course?
             it helps answer:
             “Which of the IDs the user asked to delete actually exist (and belong to the course)?”
             */

            var actualSectionIds = sections.Select(s => s.Id).ToHashSet();


            // Update course counter
            //course.NumberOfSections -= SectionList.Count;
            await _mediator.Publish(new SectionDeletedEvent(request.CourseId, 1), cancellationToken);


            // Remove sections (EF will track and delete)
            foreach (var section in sections)
            {
                await sectionRepo.RemoveAsync(section.Id, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success($"{SectionList.Count} sections deleted successfully");
        }
    }
}
