using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Api.Tests;

public class AksDiscoveryControllerTests(ConfigLensApiFactory factory) : IClassFixture<ConfigLensApiFactory>
{
    [Fact]
    public async Task GetClusters_returns_clusters_for_a_known_subscription()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/aks/clusters?subscriptionId=sub-assessor-prod-01");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var clusters = await response.Content.ReadFromJsonAsync<List<string>>();
        clusters.Should().Contain("aks-assessor-prod-eastus");
    }

    [Fact]
    public async Task GetClusters_returns_400_when_subscriptionId_is_missing()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/aks/clusters");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetClusters_without_a_token_returns_401()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/aks/clusters?subscriptionId=sub-assessor-prod-01");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetNamespaces_returns_namespaces_for_a_known_cluster()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync(
            "/api/aks/namespaces?subscriptionId=sub-assessor-prod-01&clusterName=aks-assessor-prod-eastus");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var namespaces = await response.Content.ReadFromJsonAsync<List<string>>();
        namespaces.Should().Contain("assessor-prod");
    }

    [Fact]
    public async Task GetNamespaces_returns_400_when_clusterName_is_missing()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/aks/namespaces?subscriptionId=sub-assessor-prod-01");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetNamespaces_returns_empty_for_an_unknown_cluster()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync(
            "/api/aks/namespaces?subscriptionId=sub-assessor-prod-01&clusterName=nonexistent-cluster");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var namespaces = await response.Content.ReadFromJsonAsync<List<string>>();
        namespaces.Should().BeEmpty();
    }
}
