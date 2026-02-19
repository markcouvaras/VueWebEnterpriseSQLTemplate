namespace VueWebEnterpriseSQL.Application.Common
{
    public abstract class RequestParameters
    {
        private const int MaxPageSize = 50;
        private int _page = 1;
        private int _pageSize = 10;

        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
        }

        /// <summary>
        /// Column name to sort by. Validated against an allow-list before reaching SQL.
        /// </summary>
        public string? OrderBy { get; set; }

        /// <summary>
        /// Sort direction: "asc" or "desc". Defaults to "asc".
        /// </summary>
        public string? OrderDirection { get; set; }

        /// <summary>
        /// Comma-separated list of fields to return (sparse fieldsets).
        /// Reserved for future use — ignored when null.
        /// </summary>
        public string? Fields { get; set; }
    }
}
