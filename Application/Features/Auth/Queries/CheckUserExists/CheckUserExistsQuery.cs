using Application.Features.Auth.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.Auth.Queries.CheckUserExists
{
    public class CheckUserExistsQuery : IRequest<Result<CheckUserExistsResponse?>>
    {
        public string Email { get; set; } = string.Empty;
    }
}
