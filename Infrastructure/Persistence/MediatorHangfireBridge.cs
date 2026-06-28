using MediatR;
using System.ComponentModel;

namespace Infrastructure.Persistence
{
    public class MediatorHangfireBridge(IMediator mediator)
    {
        private readonly IMediator _mediator = mediator;

        [DisplayName("Process Command: {0}")]
        public async Task SendAsync(object command)
        {
            // Hangfire will deserialize the command and this will route it to the correct handler
            await _mediator.Send(command);
        }
    }
}
