using Application.Interfaces;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class VideoAddedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<VideoAddedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public Task Handle(VideoAddedEvent notification, CancellationToken cancellationToken)
        {
            var sectionRepo = _unitOfWork.Repository<Domain.Entities.Section>();
            var sectionTask = sectionRepo.GetByIdAsync(notification.Id, cancellationToken);
            return sectionTask.ContinueWith(task =>
            {
                var section = task.Result;
                if (section != null)
                {
                    section.NumberOfVideos += notification.NumberOfVideos;
                    sectionRepo.Update(section);
                }
            }, cancellationToken);

        }
    }
}
