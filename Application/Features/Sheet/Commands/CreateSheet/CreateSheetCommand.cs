using Application.DTOs.Sheet;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Sheet.Commands.CreateSheet
{
    public class CreateSheetCommand : IRequest<Result<SheetCreationResponse>>
    {
        public string Name { get; set; } = string.Empty;
        public IFormFile SheetUrl { get; set; } = null!;
        public SheetType Type { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? VideoId { get; set; }
        public Guid InstructorId { get; set; }
    }
}
