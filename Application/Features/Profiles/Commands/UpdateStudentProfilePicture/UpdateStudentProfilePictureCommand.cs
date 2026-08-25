using Application.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Profiles.Commands.UpdateStudentProfilePicture
{
    public class UpdateStudentProfilePictureCommand : IRequest<Result<string>>
    {
        public required Guid StudentId { get; init; }
        public required IFormFile PictureFile { get; init; }
    }
}
