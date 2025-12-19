using Application.DTOs.Sections;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Sections.Commands.CreateSection
{
    public record BulkCreateSectionCommand(List<CreateSectionRequest> Sections): IRequest<Result<List<CreateSectionResponse>>>;
}
