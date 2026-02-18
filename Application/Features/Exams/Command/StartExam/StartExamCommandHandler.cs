using Application.ResultWrapper;
using Application.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Exams.Command.StartExam
{
    public class StartExamCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<StartExamCommand, Result<StartedExamResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<StartedExamResponse>> Handle(StartExamCommand request, CancellationToken cancellationToken)
        {
            var studentRepository = _unitOfWork.GetRepository<IUserRepository>();

            User? user = await studentRepository.GetStudentByIdWithRelationsAsync(request.Student, cancellationToken);

            if (user == null)
            {
                return Result<StartedExamResponse>.FailureStatusCode("Student not found", ErrorType.NotFound);
            }

            Student? Student = user.Student;

            if (Student == null)
            {
                return Result<StartedExamResponse>.FailureStatusCode("User is not a student", ErrorType.BadRequest);
            }

            var studentExamResultRepository = _unitOfWork.Repository<StudentExamResult>();
            var examRepository = _unitOfWork.Repository<Exam>();

            // Find the existing StudentExamResult (created during exam generation)
            var studentExamResult = await studentExamResultRepository
                .FirstOrDefaultAsync(ser => ser.ExamId == request.ExamId && ser.StudentId == user.Id, cancellationToken);

            if (studentExamResult == null)
            {
                return Result<StartedExamResponse>.FailureStatusCode("Exam not found for this student", ErrorType.NotFound);
            }

            // Check if exam is already in progress or completed
            if (studentExamResult.Status != ExamResultStatus.NotStarted)
            {
                return Result<StartedExamResponse>.FailureStatusCode("Exam already started or completed by the student", ErrorType.Conflict);
            }

            // Get the exam to validate timing
            var exam = await examRepository.GetByIdAsync(request.ExamId, cancellationToken);

            if (exam == null)
            {
                return Result<StartedExamResponse>.FailureStatusCode("Exam not found", ErrorType.NotFound);
            }

            // For FixedTimeExam, check if exam has started
            // For FlexibleTimeExam, students can start anytime (within the exam period)
            if (exam.ExamType == ExamType.FixedTimeExam)
            {
                if (exam.StartTime.HasValue && exam.StartTime > DateTimeOffset.UtcNow)
                {
                    return Result<StartedExamResponse>.FailureStatusCode("Exam has not started yet", ErrorType.Conflict);
                }
            }

            // Check if exam has ended (applies to both exam types)
            if (exam.EndTime.HasValue && exam.EndTime < DateTimeOffset.UtcNow)
            {
                return Result<StartedExamResponse>.FailureStatusCode("Exam has ended", ErrorType.Conflict);
            }

            // Update the existing StudentExamResult to InProgress
            studentExamResult.Status = ExamResultStatus.InProgress;
            studentExamResult.TakenAt = DateTimeOffset.UtcNow;
            studentExamResult.UpdatedAt = DateTimeOffset.UtcNow;
            studentExamResultRepository.Update(studentExamResult);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<StartedExamResponse>.Success(new StartedExamResponse
            {
                Student = user.Id,
                ExamId = request.ExamId,
                StartedAt = DateTime.UtcNow
            });
        }
    }
}
