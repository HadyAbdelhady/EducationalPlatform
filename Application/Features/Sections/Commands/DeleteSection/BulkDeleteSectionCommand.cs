using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sections.Commands.DeleteSection
{
    public record BulkDeleteSectionCommand(Guid CourseId, List<Guid> SectionIds) : IRequest<Result<string>>;
}
