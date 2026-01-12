using Application.Interfaces.BaseFilters;
using Domain.Entities;

namespace Infrastructure.Persistence.HelperFunctions
{
    public abstract class ReviewFilterRegistry<T> : IBaseFilterRegistry<T> where T : Review
    {
        public virtual Dictionary<string, Func<IQueryable<T>, string, IQueryable<T>>> Filters { get; } = new()
        {
            ["starrating"] = (q, value) =>
                q.Where(r => r.StarRating >= int.Parse(value)),

            ["studentid"] = (q, value) =>
                q.Where(r => r.StudentId == Guid.Parse(value))
        };
        public virtual Dictionary<string, Func<IQueryable<T>, bool, IOrderedQueryable<T>>> Sorts { get; } = new()
        {
            ["createdat"] = (q, desc) => desc
                ? q.OrderByDescending(r => r.CreatedAt)
                : q.OrderBy(r => r.CreatedAt),

            ["starrating"] = (q, desc) => desc
                ? q.OrderByDescending(r => r.StarRating)
                : q.OrderBy(r => r.StarRating)
        };
    }

    public class CourseReviewFilterRegistry : ReviewFilterRegistry<CourseReview>
    {
    }

    public class SectionReviewFilterRegistry : ReviewFilterRegistry<SectionReview>
    {
    }

    public class VideoReviewFilterRegistry : ReviewFilterRegistry<VideoReview>
    {
    }

    public class InstructorReviewFilterRegistry : ReviewFilterRegistry<InstructorReview>
    {
    }
}