using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
builder.Configuration.AddJsonFile("appsettings.json", optional: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false);
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);
builder.Configuration.AddEnvironmentVariables();

builder.UseSerilog();
builder.Services.AddRabbitMQ("RabbitMQ");

builder.Services.AddSingleton<Processors>();

var sqlConnectionString = builder.Configuration.GetConnectionString("DB");
if (string.IsNullOrEmpty(sqlConnectionString)) throw new Exception("Missing connection string DB");
builder.Services.AddTransient<IDbConnection>(sp => new SqlConnection(sqlConnectionString));

var cancelSource = new CancellationTokenSource();

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("PROGRAM");
logger.LogInformation("Verifying connection to the DB...");
var db = host.Services.GetRequiredService<IDbConnection>();
db.Open();
db.Close();
logger.LogInformation("Connected to DB correctly");

host.Services.GetRequiredService<Processors>().EnableProcessors(cancelSource.Token);


host.Run();


