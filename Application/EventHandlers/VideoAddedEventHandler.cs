using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class VideoAddedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<VideoAddedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(VideoAddedEvent notification, CancellationToken cancellationToken)
        {
            var sectionRepo = _unitOfWork.Repository<Section>();
            var section = await sectionRepo.GetByIdAsync(notification.Id, cancellationToken, s => s.Course!);
            if (section != null)
            {
                section.NumberOfVideos += notification.NumberOfVideos;
                section.Course!.NumberOfVideos += notification.NumberOfVideos;
                sectionRepo.Update(section);
            }
        }
    }
}
