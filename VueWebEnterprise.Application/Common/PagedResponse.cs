namespace VueWebEnterprise.Application.Common
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public MetaData MetaData { get; set; } = new();

        /// <summary>
        /// Facet totals for sidebar filters.
        /// Key = facet name (e.g. "DepartmentTotals").
        /// Value = IEnumerable&lt;TableFilterSummary&lt;string&gt;&gt; or IEnumerable&lt;TableFilterSummary&lt;int&gt;&gt;.
        /// Stored as object to support mixed value types in a single response.
        /// </summary>
        public Dictionary<string, object> AggregatedData { get; set; } = [];
    }
}
