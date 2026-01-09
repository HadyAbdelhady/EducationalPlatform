namespace Application.Interfaces.BaseFilters
{
    public interface IBaseFilterRegistry<T> where T : class
    {
        public Dictionary<string, Func<IQueryable<T>, string, IQueryable<T>>> Filters { get; }
        public Dictionary<string, Func<IQueryable<T>, bool, IOrderedQueryable<T>>> Sorts { get; }
        public IQueryable<T> ApplyFilter(IQueryable<T> query, Dictionary<string, string> incomingParams);
        public IQueryable<T> ApplySort(IQueryable<T> query, string sortBy, bool isDescending);

    }
}
