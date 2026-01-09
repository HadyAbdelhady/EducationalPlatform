namespace Application.Interfaces.BaseFilters
{
    public static class QueryableFilterExtensions
    {
        public static IQueryable<T> ApplyFilters<T>(
            this IQueryable<T> query,
            Dictionary<string, string> incomingParams,
            Dictionary<string, Func<IQueryable<T>, string, IQueryable<T>>> filters)
            where T : class
        {
            if (incomingParams == null || incomingParams.Count == 0)
            {
                return query;
            }

            foreach (var (keyRaw, value) in incomingParams)
            {
                var key = keyRaw?.ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                if (filters.TryGetValue(key, out var filterFunc))
                {
                    query = filterFunc(query, value);
                }
            }

            return query;
        }

        public static IQueryable<T> ApplySort<T>(
            this IQueryable<T> query,
            string sortBy,
            bool isDescending,
            Dictionary<string, Func<IQueryable<T>, bool, IOrderedQueryable<T>>> sorts)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query;
            }

            var key = sortBy.ToLowerInvariant();
            if (sorts.TryGetValue(key, out var sortFunc))
            {
                return sortFunc(query, isDescending);
            }

            return query;
        }
    }
}
