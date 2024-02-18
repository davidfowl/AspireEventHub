using Aspire.Hosting.Azure;

public class AzureEventHubsResource(string name, string path) :
    AzureBicepResource(name, templateFile: path),
    IResourceWithConnectionString
{
    public BicepOutputReference NamespaceEndpoint => new("serviceBusEndpoint", this);

    string IResourceWithConnectionString.ConnectionStringExpression 
        => NamespaceEndpoint.ValueExpression;

    public string? GetConnectionString() => NamespaceEndpoint.Value;
}

public static class AzureEventHubExtensions
{
    public static IResourceBuilder<AzureEventHubsResource> AddEventHubs(this IDistributedApplicationBuilder builder, string name, string[] eventHubs)
    {
        var resource = new AzureEventHubsResource(name, Path.GetFullPath("eventhub.bicep", builder.AppHostDirectory));
        return builder.AddResource(resource)
                      .WithParameter(AzureBicepResource.KnownParameters.PrincipalId)
                      .WithParameter(AzureBicepResource.KnownParameters.PrincipalType)
                      .WithParameter("eventHubNamespaceName", resource.CreateBicepResourceName())
                      .WithParameter("eventHubs", eventHubs)
                      .WithManifestPublishingCallback(resource.WriteToManifest);
    }
}
