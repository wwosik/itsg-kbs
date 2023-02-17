using ITSG.KBS.Messages;

namespace ITSG.KBS;

public interface IProcessor<TRequest, TResponse> where TResponse : IResponse, new()
{
    Task<TResponse> ProcessAsync(TRequest request, CancellationToken cancellationToken);
}

