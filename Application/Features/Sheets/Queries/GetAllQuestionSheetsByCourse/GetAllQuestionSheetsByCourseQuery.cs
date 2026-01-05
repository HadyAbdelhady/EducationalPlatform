using Application.DTOs.Sheets;
using Application.ResultWrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Sheets.Queries.GetAllQuestionSheetsByCourse
{
    public class GetAllQuestionSheetsByCourseQuery : IRequest<Result<PaginatedResult<SheetResponse>>>
    {
        public Guid CourseId { get; set; }
    }
}



