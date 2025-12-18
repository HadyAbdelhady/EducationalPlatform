using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Section.Commands.DeleteSection
{
    public record BulkDeleteSectionCommand(Guid CourseId, List<Guid> SectionIds) : IRequest<Result<string>>;
}
