namespace ITSG.KBS;

public interface IProcessor<TRequest, TResponse>
{
    Task<TResponse> ProcessAsync(TRequest request, CancellationToken cancellationToken);
}
