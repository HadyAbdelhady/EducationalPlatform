namespace Application.Interfaces.BaseFilters
{
    public interface IBaseFilterRegistry<T> where T : class
    {
        public Dictionary<string, Func<IQueryable<T>, string, IQueryable<T>>> Filters { get; }
        public Dictionary<string, Func<IQueryable<T>, bool, IOrderedQueryable<T>>> Sorts { get; }

    }
}
