using Application.Common;
using Application.Features.Sheets.DTOs;
using MediatR;

namespace Application.Features.HomeScreen.InstructorStudentSheets
{
    public class GetInstructorStudentSheetsQuery : IRequest<Result<PaginatedResult<SheetResponse>>>
    {
        public Guid InstructorId { get; set; }

        public Guid StudentId { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
