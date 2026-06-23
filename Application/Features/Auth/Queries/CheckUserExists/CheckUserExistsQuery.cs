using Application.DTOs.Auth;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Auth.Queries.CheckUserExists
{
    public class CheckUserExistsQuery : IRequest<Result<CheckUserExistsResponse?>>
    {
        public string Email { get; set; } = string.Empty;
    }
}
