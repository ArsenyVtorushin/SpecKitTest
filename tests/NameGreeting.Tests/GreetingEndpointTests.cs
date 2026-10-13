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

    private Task<HttpResponseMessage> PostJsonAsync(string body)
    {
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        return _client.PostAsync("/api/greeting", content);
    }
}
