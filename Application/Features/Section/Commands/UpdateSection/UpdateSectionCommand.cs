using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Section.Commands.UpdateSection
{
    using global::Application.DTOs.Section;
    using global::Application.ResultWrapper;
    using MediatR;

    namespace Application.Features.Section.Commands.UpdateSection
    {
        public record UpdateSectionCommand : IRequest<Result<SectionUpdateResponse>>
        {
            public Guid SectionId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public Guid CourseId { get; set; }
        }
    }

}
