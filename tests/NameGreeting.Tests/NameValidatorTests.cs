namespace NameGreeting.Tests;

public class NameValidatorTests
{
    [Theory]
    [InlineData("Ada", "Ada")]
    [InlineData("  Ada  ", "Ada")]
    [InlineData("Zoë", "Zoë")]
    [InlineData("O'Brien", "O'Brien")]
    [InlineData("Jean-Luc", "Jean-Luc")]
    [InlineData("Анна", "Анна")]
    [InlineData("<b>Bob</b>", "<b>Bob</b>")]
    public void Validate_ValidName_ReturnsTrimmedName(string rawName, string expectedName)
    {
        var result = NameValidator.Validate(rawName);

        Assert.True(result.IsValid);
        Assert.Null(result.Error);
        Assert.Equal(expectedName, result.CleanedName);
    }

    [Fact]
    public void Validate_NameOfExactly100Characters_IsValid()
    {
        var name = new string('a', 100);

        var result = NameValidator.Validate(name);

        Assert.True(result.IsValid);
        Assert.Null(result.Error);
        Assert.Equal(name, result.CleanedName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n")]
    public void Validate_MissingOrBlankName_ReturnsEnterNameError(string? rawName)
    {
        var result = NameValidator.Validate(rawName);

        Assert.False(result.IsValid);
        Assert.Equal("Please enter your name.", result.Error);
    }

    [Fact]
    public void Validate_NameOf101Characters_ReturnsTooLongError()
    {
        var result = NameValidator.Validate(new string('a', 101));

        Assert.False(result.IsValid);
        Assert.Equal("Name must be 100 characters or fewer.", result.Error);
    }

    // "e" followed by a combining acute accent is two chars in C# but one character
    // a person sees, so the length limit must count it once (research.md R4).
    private const string EWithCombiningAccent = "é";

    [Fact]
    public void Validate_100CombinedCharacters_IsValid()
    {
        var name = string.Concat(Enumerable.Repeat(EWithCombiningAccent, 100));

        var result = NameValidator.Validate(name);

        Assert.True(result.IsValid);
        Assert.Equal(name, result.CleanedName);
    }

    [Fact]
    public void Validate_101CombinedCharacters_ReturnsTooLongError()
    {
        var name = string.Concat(Enumerable.Repeat(EWithCombiningAccent, 101));

        var result = NameValidator.Validate(name);

        Assert.False(result.IsValid);
        Assert.Equal("Name must be 100 characters or fewer.", result.Error);
    }
}
