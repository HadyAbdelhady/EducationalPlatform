using Application.Common;
using Application.Features.Students.DTOs;
using MediatR;

namespace Application.Features.Students.Commands.IncrementScreenshotTrial
{
    public class IncrementScreenshotTrialCommand : IRequest<Result<StudentScreenshotStatusDto>>
    {
        public Guid StudentId { get; set; }
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public string? PageName { get; set; }
    }
}
