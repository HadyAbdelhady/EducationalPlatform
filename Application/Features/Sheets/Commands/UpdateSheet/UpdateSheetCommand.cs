using Application.DTOs.Sheets;
using Application.ResultWrapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Sheets.Commands.UpdateSheet
{
    public class UpdateSheetCommand : IRequest<Result<SheetUpdateResponse>>
    {
        public Guid SheetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public IFormFile? SheetUrl { get; set; }
        public DateTimeOffset? DueDate { get; set; }
    }
}
