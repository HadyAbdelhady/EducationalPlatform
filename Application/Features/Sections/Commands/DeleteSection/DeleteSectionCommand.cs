
    using Application.ResultWrapper;
    using MediatR;

    namespace Application.Features.Sections.Commands.DeleteSection
    {
        public record DeleteSectionCommand: IRequest<Result<string>>
        {
            public Guid SectionId { get;set; }
        }
    }
