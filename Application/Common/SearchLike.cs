namespace Application.Common
{
    public static class SearchLike
    {
        public static string ContainsPattern(string term)
        {
            var escaped = term
                .Replace("\\", "\\\\", StringComparison.Ordinal)
                .Replace("%", "\\%", StringComparison.Ordinal)
                .Replace("_", "\\_", StringComparison.Ordinal);

            return $"%{escaped}%";
        }
    }
}
