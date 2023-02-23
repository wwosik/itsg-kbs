global using System;
global using FluentMigrator;
global using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using System.Linq;

if (args.Length == 0)
{
    Console.WriteLine("Specify up [migration] or down <migration>");
    return;
}

bool isUpMode;
int migrationNumber = -1;

switch (args[0])
{
    case "up":
        isUpMode = true;
        if (args.Length >= 2 && !int.TryParse(args[1], out migrationNumber))
        {
            Console.WriteLine("invalid migration number");
            return;
        }
        break;
    case "down":
        isUpMode = false;
        if (args.Length < 2 || !int.TryParse(args[1], out migrationNumber))
        {
            Console.WriteLine("down must be specified with a valid migration number");
            return;
        }
        break;
    default:
        Console.WriteLine("Unknown command. Specify up [migration] or down <migration>");
        return;
}


var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DB");
if (string.IsNullOrEmpty(connectionString))
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

if (args.Any(a => a == "--dev"))
{
    System.Console.WriteLine("Adding DEV");
    serviceCollection.Configure<RunnerOptions>(o =>
    {
        o.IncludeUntaggedMigrations = true;
        o.Tags = new[] { "dev" };
    });
}

var sp = serviceCollection.BuildServiceProvider(validateScopes: false); // validateScopes according to FluentMigration docs
var runner = sp.GetRequiredService<IMigrationRunner>();


if (isUpMode)
{
    if (migrationNumber >= 0)
    {
        runner.MigrateUp(migrationNumber);
    }
    else
    {
        runner.MigrateUp();
    }
}
else
{
    runner.MigrateDown(migrationNumber);
}