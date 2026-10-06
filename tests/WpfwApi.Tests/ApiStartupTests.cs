using System.Net;
using System.Text.Json;
using WpfwApi.Tests.Infrastructure;

namespace WpfwApi.Tests;

public sealed class ApiStartupTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task SwaggerDocument_IsAvailableInDevelopment()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        Assert.Equal("3.0.4", document.RootElement.GetProperty("openapi").GetString());
    }
}
