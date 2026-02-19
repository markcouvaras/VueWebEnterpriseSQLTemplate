namespace VueWebEnterpriseSQL.Application.Common
{
    public class UserParameters : RequestParameters
    {
        /// <summary>
        /// Free-text search. Applied as ILIKE on FullName and Email.
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Multi-select facet filter — only return users in these departments.
        /// Bind from query string: ?departments=Engineering&amp;departments=Marketing
        /// </summary>
        public List<string>? Departments { get; set; }

        /// <summary>
        /// Multi-select facet filter — only return users with these statuses.
        /// Bind from query string: ?statuses=1&amp;statuses=2
        /// </summary>
        public List<int>? Statuses { get; set; }
    }
}
