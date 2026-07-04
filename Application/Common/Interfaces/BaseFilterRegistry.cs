namespace Application.Common.Interfaces
{
    public interface IBaseFilterRegistry<T> where T : class
    {
        public Dictionary<string, Func<IQueryable<T>, string, IQueryable<T>>> Filters { get; }
        public Dictionary<string, Func<IQueryable<T>, bool, IOrderedQueryable<T>>> Sorts { get; }

    }
}
