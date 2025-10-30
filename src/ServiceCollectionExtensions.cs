using Amazon.DynamoDBv2;
using DynamoDB.Net.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DynamoDB.Net;

/// <summary>
/// Extension methods for registering DynamoDB-related services into an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the DynamoDB client and related services into the provided <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The IServiceCollection to register services into.</param>
    /// <param name="configureOptions">The action used to configure <see cref="DynamoDBClientOptions" />.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddDynamoDBClient(this IServiceCollection services, Action<DynamoDBClientOptions>? configureOptions = null)
    {
        services.AddOptions();
        services.AddLogging();

        services.TryAddAWSService<IAmazonDynamoDB>();
        services.TryAddSingleton<IDynamoDBSerializer, DynamoDBSerializer>();

        services.AddScoped<IDynamoDBClient, DynamoDBClient>();
        services.TryAddSingleton<IDynamoDBItemEventHandler, VersionChecker>();

        if (configureOptions != null)
            services.Configure(configureOptions);

        return services;
    }
}
