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

namespace Application.Features.Sheet.Commands.UpdateSheet
{
    public class UpdateSheetCommand : IRequest<Result<SheetUpdateResponse>>
    {
        public Guid SheetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public IFormFile? SheetUrl { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
