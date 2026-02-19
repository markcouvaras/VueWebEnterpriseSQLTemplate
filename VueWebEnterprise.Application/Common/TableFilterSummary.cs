namespace VueWebEnterprise.Application.Common
{
    /// <summary>
    /// A single facet bucket for sidebar filters.
    /// Use TableFilterSummary&lt;string&gt; for text facets (e.g. Department)
    /// and TableFilterSummary&lt;int&gt; for numeric facets (e.g. Status 0/1).
    /// </summary>
    public class TableFilterSummary<T>
    {
        public T Value { get; set; } = default!;
        public int Count { get; set; }
    }
}
