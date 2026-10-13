using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace NameGreeting.Tests;

public class GreetingEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public GreetingEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("  Ada  ", "Ada")]
    [InlineData("<b>Bob</b>", "<b>Bob</b>")]
    [InlineData("Zoë", "Zoë")]
    public async Task PostGreeting_ValidName_ReturnsCleanedName(string submittedName, string expectedName)
    {
        var response = await PostJsonAsync(JsonSerializer.Serialize(new { name = submittedName }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Read the raw JSON (not a deserialized record) so the test also checks
        // that the property is named "name" in camelCase, as the contract requires.
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(expectedName, json.RootElement.GetProperty("name").GetString());
    }

    [Theory]
    [InlineData("""{"name":""}""")]
    [InlineData("""{"name":"   "}""")]
    [InlineData("""{}""")]
    public async Task PostGreeting_MissingOrBlankName_ReturnsEnterNameError(string body)
    {
        var response = await PostJsonAsync(body);

        await AssertBadRequestWithErrorAsync(response, "Please enter your name.");
    }

    [Fact]
    public async Task PostGreeting_NameTooLong_ReturnsTooLongError()
    {
        var response = await PostJsonAsync(JsonSerializer.Serialize(new { name = new string('a', 101) }));

        await AssertBadRequestWithErrorAsync(response, "Name must be 100 characters or fewer.");
    }

    [Fact]
    public async Task PostGreeting_InvalidJson_ReturnsBadRequest()
    {
        var response = await PostJsonAsync("not json");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task AssertBadRequestWithErrorAsync(HttpResponseMessage response, string expectedError)
    {
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(expectedError, json.RootElement.GetProperty("error").GetString());
    }

    private Task<HttpResponseMessage> PostJsonAsync(string body)
    {
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        return _client.PostAsync("/api/greeting", content);
    }
}
