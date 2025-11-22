using Application.DTOs.Review;
using Application.ResultWrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Review.Commands.UpdateReview
{
    public class UpdateCourseReviewCommand : IRequest<Result<CourseReviewUpdateResponse>>
    {
        public Guid CourseReviewId { get; set; }
        public int StarRating { get; set; }
        public string? Comment { get; set; }
    }
}
