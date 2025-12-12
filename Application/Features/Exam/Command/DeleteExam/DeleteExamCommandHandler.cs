using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Exam.Command.DeleteExam
{
    public class DeleteExamCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteExamCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<string>> Handle(DeleteExamCommand request, CancellationToken cancellationToken)
        {
            var ExamRepo = _unitOfWork.Repository<Domain.Entities.Exam>();

            var exam = await ExamRepo.GetByIdAsync(request.ExamId, cancellationToken);

            if (exam is null)
            {
                return Result<string>.FailureStatusCode("Question does not exist.", ErrorType.NotFound);
            }

            var relativeEntities = await ExamRepo.GetByIdAsync(request.ExamId,
                                                        cancellationToken,
                                                        c => c.ExamResults,
                                                        c => c.StudentExams,
                                                        c => c.InstructorExams,
                                                        c => c.ExamQuestions);

            if (relativeEntities is null)
                return Result<string>.Success("Exam does not exist");

            ExamRepo.Remove(exam);

            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success("Successfully deleted the exams and its relations");
        }
    }
}
