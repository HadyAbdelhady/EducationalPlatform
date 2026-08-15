using Application.Common;
namespace Application.Common
{

    public class GetAllEntityRequestSkeleton
    {
        public Dictionary<string, string> Filters { get; set; } = [];
        public string SortBy { get; set; } = "createdat";
        public bool IsDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
