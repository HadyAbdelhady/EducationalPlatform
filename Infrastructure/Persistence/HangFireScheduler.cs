using Application.Interfaces;
using Hangfire;

namespace Infrastructure.Persistence
{
    public class HangfireScheduler : IScheduler
    {
        public Task ScheduleAsync<TRequest>(TRequest command, DateTimeOffset executionTime, CancellationToken cancellationToken = default)
        {
            // Hangfire serializes the command and stores it in the DB
            BackgroundJob.Schedule<MediatorHangfireBridge>(x => x.SendAsync(command!), executionTime);
            return Task.CompletedTask;
        }
    }
}
