using Application.HelperFunctions;
using Application.Interfaces;
using Application.DTOs.Exam;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.EventHandlers
{
    public class ExamFinishedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ExamFinishedEvent>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(ExamFinishedEvent notification, CancellationToken cancellationToken)
        {
            // Get the exam result with student submissions
            var examResultRepo = _unitOfWork.Repository<StudentExamResult>();
            var examResult = await examResultRepo.FirstOrDefaultAsync(
                predicate: er => er.Id == notification.ExamResultId,
                cancellationToken: cancellationToken,
                includes: er => er.StudentSubmissions
            );

            if (examResult == null)
            {
                return;
            }

            // Get the exam with questions and answers
            var examRepository = _unitOfWork.GetRepository<IExamRepository>();
            var examModelAnswer = await examRepository.GetExamWithQuestionsAndAnswersByIdAsync(notification.ExamId, cancellationToken);

            if (examModelAnswer == null)
            {
                return;
            }

            // Calculate obtained marks using student submissions
            var obtainedMarks = CalculateObtainedMarks.Calculate(examModelAnswer, examResult.StudentSubmissions);

            // Update the exam result with calculated marks
            examResult.StudentMark = obtainedMarks;
            examResultRepo.Update(examResult);

            //await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

