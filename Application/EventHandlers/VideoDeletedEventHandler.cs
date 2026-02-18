using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class VideoDeletedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<VideoDeletedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(VideoDeletedEvent notification, CancellationToken cancellationToken)
        {
            var sectionRepo = _unitOfWork.Repository<Section>();
            var courseRepo = _unitOfWork.Repository<Course>();

            // Load section with course relationship
            var section = await sectionRepo.GetByIdAsync(notification.SectionId, cancellationToken, s => s.Course!);
            if (section != null)
            {
                section.NumberOfVideos--;
                sectionRepo.Update(section);

                // Update course if it exists
                if (section.Course != null)
                {
                    section.Course.NumberOfVideos--;
                    courseRepo.Update(section.Course);
                }
            }
        }
    }
}

