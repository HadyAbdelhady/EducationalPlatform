using Application.Common;
namespace Application.Features.Exams.DTOs
{
    public class GetAllExamsRequest
    {
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
        public Guid UserId { get; set; }
    }
}
