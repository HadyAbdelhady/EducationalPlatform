using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Review
{
    public class CourseReviewCreationRequest
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }

        [Range(1,5,ErrorMessage ="Rate Must be between 1 and 5.")]
        public int StarRating { get; set; }
        public string? Comment { get; set; }
    }
}
