using EasyNetQ;

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
        System.Console.WriteLine(typeDef.Name == "IProcessor'2");
        System.Console.WriteLine(typeDef.Namespace);
        return typeDef.Name == "IProcessor`2" && typeDef.Namespace == "ITSG.KBS";
    }

    public void EnableProcessor<TProcessor, TRequest, TResponse>(CancellationToken cancellationToken) where TProcessor : IProcessor<TRequest, TResponse>
    {
        this.logger.LogInformation("Enabled processing of '{request}' by '{processor}'", typeof(TRequest).Name, typeof(TProcessor).Name);
        bus.Rpc.RespondAsync<TRequest, TResponse>((req, token) =>
           {
               var processor = (IProcessor<TRequest, TResponse>)ActivatorUtilities.CreateInstance<TProcessor>(this.serviceProvider);
               return processor.ProcessAsync(req, token);
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