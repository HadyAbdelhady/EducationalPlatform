namespace Application.Interfaces
{
    public interface IScheduler
    {
        Task ScheduleAsync<TRequest>(TRequest command, DateTimeOffset executionTime, CancellationToken cancellationToken = default);
    }
}
