using Application.DTOs.Review;
using Application.ResultWrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Review.Commands.CreateReview
{
    public class CreateReviewCommand : IRequest<Result<ReviewResponse>>
    {
        public Guid StudentId { get; set; }
        public Guid EntityId { get; set; }
        public int StarRating { get; set; }
        public string? Comment { get; set; }
    }
}
