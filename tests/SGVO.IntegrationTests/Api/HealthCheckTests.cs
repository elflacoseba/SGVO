using Microsoft.AspNetCore.Mvc.Testing;

namespace SGVO.IntegrationTests.Api;

/// <summary>
/// Tests de integración para el endpoint de health check.
/// </summary>
public class HealthCheckTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthCheckTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Health_Should_Return_200_And_Healthy_Status()
    {
        var response = await _client.GetAsync("/api/health");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Healthy", content);
    }
}
