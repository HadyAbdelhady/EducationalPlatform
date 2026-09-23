using Application.Common;
using Application.Features.Students.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Students.Commands.IncrementScreenshotTrial
{
    public class IncrementScreenshotTrialCommand : IRequest<Result<StudentScreenshotStatusDto>>
    {
        public Guid StudentId { get; set; }
        public IFormFile ImageFile { get; set; } = null!;
        public string? PageName { get; set; }
    }
}
