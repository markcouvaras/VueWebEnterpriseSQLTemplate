using FluentValidation.Results;

namespace VueWebEnterprise.Application.Common.Exceptions
{
    /// <summary>
    /// Custom validation exception that holds a dictionary of property → error messages.
    /// Thrown by the ValidationBehaviour when FluentValidation rules fail.
    /// </summary>
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
    }
}
