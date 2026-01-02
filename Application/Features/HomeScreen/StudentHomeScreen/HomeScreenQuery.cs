using Application.DTOs.HomeScreen;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.HomeScreen.StudentHomeScreen
{
    public class HomeScreenQuery : IRequest<Result<StudentHomeScreenResponse>>
    {
        public Guid StudentId { get; set; }
    }
}
