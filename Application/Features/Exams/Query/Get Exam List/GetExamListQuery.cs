using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Exam;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Exams.Query.Get_Exam_List
{
    public class GetAllExamsQuery : IRequest<Result<PaginatedResult<ExamListDto>>>
    {
        public Dictionary<string, string> Filters { get; set; } = [];
        public string SortBy { get; set; } = "createdat";
        public bool IsDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public Guid UserId { get; set; }
    }


    public class GetAllExamsRequest
    {
        public Dictionary<string, string> Filters { get; set; } = [];
        public string SortBy { get; set; } = "createdat";
        public bool IsDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
    }



}