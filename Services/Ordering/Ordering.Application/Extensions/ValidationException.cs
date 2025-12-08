using FluentValidation.Results;

namespace Ordering.Application.Extensions;

public class ValidationException:ApplicationException
{
    public Dictionary<string, string[]> Errors { get; set; }
    public ValidationException():base("One or more validation error(s) occurred")
    {
        Errors = new Dictionary<string, string[]>();
    }
    public ValidationException(IEnumerable<ValidationFailure> failures):this()
    {
        Errors = failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(k => k.Key, k => k.ToArray());
    }
}