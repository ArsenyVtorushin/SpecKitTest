// The outcome of checking a submitted name: either a cleaned name, or an error message
// the page can show to the visitor as-is.
public record NameValidationResult(string? CleanedName, string? Error)
{
    public bool IsValid => Error is null;
}

public static class NameValidator
{
    public static NameValidationResult Validate(string? rawName)
    {
        // A missing name is handled the same way as an empty one.
        var cleanedName = (rawName ?? "").Trim();

        return new NameValidationResult(cleanedName, Error: null);
    }
}
