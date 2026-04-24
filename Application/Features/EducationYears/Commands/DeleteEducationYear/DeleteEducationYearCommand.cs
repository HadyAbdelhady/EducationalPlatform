using Application.ResultWrapper;
using MediatR;

namespace Application.Features.EducationYears.Commands.DeleteEducationYear
{
    public class DeleteEducationYearCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
    }
}
