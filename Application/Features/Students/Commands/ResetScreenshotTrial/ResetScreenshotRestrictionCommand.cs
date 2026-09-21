using Application.Common;
using Application.Features.Students.DTOs;
using MediatR;

namespace Application.Features.Students.Commands.ResetScreenshotTrial
{
    public class ResetScreenshotRestrictionCommand : IRequest<Result<StudentScreenshotStatusDto>>
    {
        public Guid StudentId { get; set; }
    }
}
