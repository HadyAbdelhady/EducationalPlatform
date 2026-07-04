using Application.Features.Reviews.DTOs;
using Application.Common;
using Domain.enums;
using MediatR;

namespace Application.Features.Reviews.Query.CheckReviewExists
{
    public class CheckReviewExistsQuery : IRequest<Result<ReviewResponse?>>
    {
        public Guid EntityId { get; set; }
        public ReviewEntityType EntityType { get; set; }
        public Guid StudentId { get; set; }
    }
}
