using Application.DTOs.EducationYear;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.EducationYears.Queries.GetEducationYearById
{
    public class GetEducationYearByIdQuery : IRequest<Result<EducationYearResponse>>
    {
        public Guid Id { get; set; }
    }
}
