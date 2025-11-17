using Application.DTOs.Section;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Section.Commands.CreateSection
{
    public record BulkCreateSectionCommand(List<CreateSectionRequest> Sections): IRequest<Result<List<CreateSectionResponse>>>;
}
