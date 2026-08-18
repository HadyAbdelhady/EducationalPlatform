using Application.Common;
using Application.Features.Search.DTOs;
using MediatR;

namespace Application.Features.Search.Queries.StudentSearch
{
    public class StudentSearchQuery : IRequest<Result<StudentSearchResponse>>
    {
        public Guid UserId { get; set; }

        public string Query { get; set; } = string.Empty;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
