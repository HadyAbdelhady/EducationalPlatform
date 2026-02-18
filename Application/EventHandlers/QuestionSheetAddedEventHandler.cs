using Application.Interfaces;
using Domain.Entities;
using Domain.enums;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class QuestionSheetAddedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<QuestionSheetAddedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(QuestionSheetAddedEvent notification, CancellationToken cancellationToken)
        {
            if (notification.EntityType == EntityType.None)
            {
                return;
            }
            if (notification.EntityType == EntityType.Section)
            {
                var sectionRepo = _unitOfWork.Repository<Section>();
                var section = await sectionRepo.GetByIdAsync(notification.EntityId, cancellationToken, sc => sc.Course!);
                if (section != null)
                {
                    section.NumberOfQuestionSheets += 1;
                    section.Course!.NumberOfQuestionSheets += 1;
                    sectionRepo.Update(section);
                }
                return;
            }

            var courseRepo = _unitOfWork.Repository<Course>();
            var course = await courseRepo.GetByIdAsync(notification.EntityId, cancellationToken);
            if (course != null)
            {
                course.NumberOfQuestionSheets += 1;
                courseRepo.Update(course);
            }
        }
    }
}
