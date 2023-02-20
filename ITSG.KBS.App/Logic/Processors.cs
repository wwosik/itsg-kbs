using ITSG.KBS.Messages;
using ITSG.KBS.Messages.Config;

namespace ITSG.KBS;

public class Processors
{
    private readonly IServiceProvider serviceProvider;
    private readonly IBus bus;
    private readonly ILogger<Processors> logger;

    public Processors(IServiceProvider serviceProvider, EasyNetQ.IBus bus, ILogger<Processors> logger)
    {
        this.serviceProvider = serviceProvider;
        this.bus = bus;
        this.logger = logger;
    }

    private static bool IsProcessorType(Type i)
    {
        if (!i.IsGenericType) return false;
        var typeDef = i.GetGenericTypeDefinition();
        return typeDef.Name == "IProcessor`2" && typeDef.Namespace == "ITSG.KBS";
    }

    public void EnableProcessor<TProcessor, TRequest, TResponse>(CancellationToken cancellationToken) where TProcessor : IProcessor<TRequest, TResponse> where TResponse : IResponse, new()
    {
        this.logger.LogDebug("Enabling processing of '{request}' by '{processor}'...", typeof(TRequest).Name, typeof(TProcessor).Name);
        try
        {
            bus.Rpc.RespondAsync<TRequest, TResponse>(async (req, token) =>
           {
               this.logger.LogDebug("Message {msg} received", typeof(TRequest));

               try
               {
                   var processor = (IProcessor<TRequest, TResponse>)ActivatorUtilities.CreateInstance<TProcessor>(this.serviceProvider);
                   return await processor.ProcessAsync(req, token);
               }
               catch (Exception ex)
               {
                   var message = ex.GetBaseException().Message;
                   this.logger.LogError(ex, "Processor error: {message}", message);
                   return new TResponse { Error = message };
               }
           }, o => { }, cancellationToken).GetAwaiter().GetResult();

            this.logger.LogInformation("Enabled processing of '{request}' by '{processor}'.", typeof(TRequest).Name, typeof(TProcessor).Name);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to connect to RabbitMQ.");
            throw;
        }
    }

    public void EnableProcessors(CancellationToken cancellationToken)
    {
        var processorTypes = Assembly.GetExecutingAssembly().GetTypes()
            .SelectMany(type =>
            {
                var interfaces = type.GetInterfaces();
                return interfaces.Where(IsProcessorType).Select(i => (Type: type, i.GenericTypeArguments));
            });

        var enableProcessorMethodDefinition = this.GetType().GetMethods().First(r => r.Name == "EnableProcessor");

        foreach (var p in processorTypes)
        {
            var respondMethod = enableProcessorMethodDefinition.MakeGenericMethod(p.Type, p.GenericTypeArguments[0], p.GenericTypeArguments[1]);
            respondMethod.Invoke(this, new object[] { cancellationToken });
        }
    }
}