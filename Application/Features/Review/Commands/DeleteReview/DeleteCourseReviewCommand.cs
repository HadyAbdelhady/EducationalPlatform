using Application.DTOs.Review;
using Application.ResultWrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Review.Commands.DeleteReview
{
    public class DeleteCourseReviewCommand : IRequest<Result<string>>
    {
        public Guid CourseReviewId { get; set; }
    }
}
