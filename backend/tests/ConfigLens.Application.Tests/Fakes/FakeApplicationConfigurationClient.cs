using ConfigLens.Domain.Configuration;

namespace ConfigLens.Application.Tests.Fakes;

public sealed class FakeApplicationConfigurationClient : IApplicationConfigurationClient
{
    private readonly Dictionary<(string App, string Env, string Tenant, int Year), ConfigTree> _trees = [];

    public FakeApplicationConfigurationClient With(string application, string environment, string tenant, int sessionYear, ConfigTree tree)
    {
        _trees[(application, environment, tenant, sessionYear)] = tree;
        return this;
    }

    public ConfigTree? GetConfiguration(ApplicationConfigurationQuery query) =>
        _trees.GetValueOrDefault((query.Application, query.Environment, query.Tenant, query.SessionYear));
}
