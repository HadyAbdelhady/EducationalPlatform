using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Auth.Commands.GoogleLogout
{
    /// <summary>
    /// Command for logging out a user (Google or any external auth).
    /// This is a client-side operation that invalidates the session.
    /// </summary>
    public class GoogleLogoutCommand : IRequest<Result<bool>>
    {
        /// <summary>
        /// User ID to logout.
        /// </summary>
        public Guid UserId { get; set; }
    }
}
