using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Review
{
    public class CourseReviewCreationResponse
    {
        public Guid CourseReviewId { get; set; }
        public int StarRating { get; set; }
        public string? Comment { get; set; }
    }
}
