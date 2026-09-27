using System.Net;
using System.Net.Http.Json;

namespace LibraryApi.Tests;

public sealed class HealthCheckTests
{
    [Fact]
    public async Task GetHealthCheck_ReturnsOkStatus()
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/v1/health-check");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<HealthCheckResponse>();
        Assert.NotNull(body);
        Assert.Equal("ok", body.Status);
    }

    private sealed record HealthCheckResponse(string Status);
}
