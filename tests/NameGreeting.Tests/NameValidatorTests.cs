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
}
