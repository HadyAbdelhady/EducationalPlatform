namespace Application.ResultWrapper
{
    public class PaginatedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public int TotalCount { get; init; } 
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
