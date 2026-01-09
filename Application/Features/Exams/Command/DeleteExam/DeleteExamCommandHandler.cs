using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.Features.Exams.Command.DeleteExam
{
    public class DeleteExamCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<DeleteExamCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;


        public async Task<Result<string>> Handle(DeleteExamCommand request, CancellationToken cancellationToken)
        {
            var ExamRepo = _unitOfWork.Repository<Exam>();
            var StudentSubmissionRepo = _unitOfWork.Repository<StudentSubmission>();


            var exam = await ExamRepo.GetByIdAsync(request.ExamId, cancellationToken);

            if (exam is null)
            {
                return Result<string>.Success("Exam does not exist");
            }

            var ExamSubmissions =  StudentSubmissionRepo
                                                                .Find(ss => ss.ExamResult.ExamId == request.ExamId,
                                                                cancellationToken);



            var relativeEntities = await ExamRepo.GetByIdAsync(request.ExamId,
                                                        cancellationToken,
                                                        c => c.ExamResults,
                                                        c => c.StudentExams,
                                                        c => c.InstructorExams,
                                                        c => c.ExamQuestions);

            exam.IsDeleted = true;

            foreach (var result in exam.ExamResults) result.IsDeleted = true;
            foreach (var bank in exam.ExamQuestions) bank.IsDeleted = true;
            foreach (var studentExam in exam.StudentExams) studentExam.IsDeleted = true;
            foreach (var instructorExam in exam.InstructorExams) instructorExam.IsDeleted = true;
            foreach (var submission in ExamSubmissions) submission.IsDeleted = true;

            await _mediator.Publish(new ExamDeletedEvent(request.CourseId, request.SectionId), cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Successfully deleted the exams and its relations");
        }
    }
}
