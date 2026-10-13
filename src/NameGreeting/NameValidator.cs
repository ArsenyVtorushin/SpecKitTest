using System.Globalization;

// The outcome of checking a submitted name: either a cleaned name, or an error message
// the page can show to the visitor as-is.
public record NameValidationResult(string? CleanedName, string? Error)
{
    public bool IsValid => Error is null;
}

public static class NameValidator
{
    public const int MaxNameLength = 100;

    public static NameValidationResult Validate(string? rawName)
    {
        // A missing name is handled the same way as an empty one.
        var cleanedName = (rawName ?? "").Trim();

        if (cleanedName.Length == 0)
        {
            return new NameValidationResult(CleanedName: null, "Please enter your name.");
        }

        // string.Length counts UTF-16 code units, so "é" written as e + accent would count
        // as 2. Counting text elements matches the characters a visitor actually sees.
        if (new StringInfo(cleanedName).LengthInTextElements > MaxNameLength)
        {
            return new NameValidationResult(CleanedName: null, $"Name must be {MaxNameLength} characters or fewer.");
        }

        return new NameValidationResult(cleanedName, Error: null);
    }
}
