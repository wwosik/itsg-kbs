global using System;
global using FluentMigrator;
global using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DB");
if (string.IsNullOrEmpty("DB"))
{
    System.Console.WriteLine("Connection string DB fehlt!");
    Environment.Exit(1);
}

var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging(lb => lb.AddFluentMigratorConsole());

serviceCollection.AddFluentMigratorCore().ConfigureRunner(rb => rb.AddSqlServer()
                                .WithGlobalConnectionString(connectionString)
                                .ScanIn(Assembly.GetExecutingAssembly()).For.All()
                                );

if (args.Length >= 1)
{
    serviceCollection.Configure<RunnerOptions>(o =>
    {
        o.IncludeUntaggedMigrations = true;
        o.Tags = new[] { args[0] };
    });
}

var sp = serviceCollection.BuildServiceProvider(validateScopes: false); // validateScopes according to FluentMigration docs
var runner = sp.GetRequiredService<IMigrationRunner>();



runner.MigrateUp();
//runner.MigrateDown(0);
