using ITSG.KBS.Messages;

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
        this.logger.LogInformation("Enabled processing of '{request}' by '{processor}'", typeof(TRequest).Name, typeof(TProcessor).Name);
        bus.Rpc.RespondAsync<TRequest, TResponse>(async (req, token) =>
           {
               try
               {
                   var processor = (IProcessor<TRequest, TResponse>)ActivatorUtilities.CreateInstance<TProcessor>(this.serviceProvider);
                   return await processor.ProcessAsync(req, token);
               }
               catch (Exception ex)
               {
                   var message = ex.GetBaseException().Message;
                   this.logger.LogError(ex, "{message}", message);
                   return new TResponse { Error = message };
               }
           }, o => { }, cancellationToken);
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