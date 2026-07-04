using Application.Features.HomeScreen.DTOs;
using Application.Common;
using MediatR;

namespace Application.Features.HomeScreen.StudentHomeScreen
{
    public class HomeScreenQuery : IRequest<Result<StudentHomeScreenResponse>>
    {
        public Guid StudentId { get; set; }
    }
}
