using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Section.Commands.DeleteSection
{
    public record BulkDeleteSectionCommand(List<Guid> SectionIds): IRequest<Result<string>>;
}
