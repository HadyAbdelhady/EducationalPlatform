using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.ReviewService
{
    public class ReviewServiceFactory : IReviewServiceFactory
    {
        public IReviewService GetReviewService(string reviewType)
        {
            switch (reviewType)
            {
                case "Course" :
                    return new CourseReviewService();
                    break;

                default:
                    break;
            }
        }
    }
}
