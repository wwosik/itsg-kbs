namespace ITSG.KBS.App.Test;

using FluentMigrator.Runner;
using MartinCostello.SqlLocalDb;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;

public class TestServiceProvider : IServiceProvider, IDisposable
{
    private readonly IServiceProvider serviceProvider;

    public TestServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(sp => new SqlLocalDbApi(sp.GetRequiredService<ILoggerFactory>()));
        services.AddSingleton<TemporarySqlLocalDbInstance>(sp => sp.GetRequiredService<SqlLocalDbApi>().CreateTemporaryInstance());
        services.AddTransient<IDbConnection>(sp =>
        {
            var instance = sp.GetRequiredService<TemporarySqlLocalDbInstance>();
            System.Console.WriteLine(instance.ConnectionString);
            return new SqlConnection(instance.ConnectionString);
        });
        services.AddFluentMigratorCore().ConfigureRunner(rb => rb.AddSqlServer()
                                    .WithGlobalConnectionString(sp => sp.GetRequiredService<TemporarySqlLocalDbInstance>().ConnectionString)
                                    .ScanIn(typeof(ITSG.KBS.Migrations.M_001_Organisations).Assembly).For.All()
                                    );

        this.serviceProvider = services.BuildServiceProvider();

        var runner = this.serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    public void Dispose()
    {
        (this.serviceProvider as IDisposable)?.Dispose();
    }

    public object? GetService(Type serviceType)
    {
        return this.serviceProvider.GetService(serviceType);
    }
}

[CollectionDefinition("Tests with service provider")]
public class TestServiceProviderFixture : ICollectionFixture<TestServiceProvider> { }