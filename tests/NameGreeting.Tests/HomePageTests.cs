using Microsoft.AspNetCore.Mvc.Testing;

namespace NameGreeting.Tests;

public class HomePageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HomePageTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRoot_ReturnsPageWithGreetingForm()
    {
        var response = await _client.GetAsync("/");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("id=\"greeting-form\"", body);
        Assert.Contains("id=\"name-input\"", body);
        Assert.Contains("id=\"submit-button\"", body);
        Assert.Contains("aria-live=\"polite\"", body);
    }
}
