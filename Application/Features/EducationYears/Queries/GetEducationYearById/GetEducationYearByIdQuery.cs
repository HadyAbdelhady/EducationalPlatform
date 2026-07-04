using Application.Features.EducationYears.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.EducationYears.Queries.GetEducationYearById
{
    public class GetEducationYearByIdQuery : IRequest<Result<EducationYearResponse>>
    {
        public Guid Id { get; set; }
    }
}
