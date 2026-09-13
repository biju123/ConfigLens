namespace ConfigLens.Domain.Scan.Dependency;

public enum DependencyType
{
    AzureSql,
    AzureTableStorage,
    AzureBlobStorage,
    AzureServiceBus,
    AzureKeyVault,
    ExternalApi
}
