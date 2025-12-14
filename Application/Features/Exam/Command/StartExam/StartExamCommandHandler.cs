using Application.ResultWrapper;
using Application.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Exam.Command.StartExam
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

            if (Student.StudentExams.Any(se => se.ExamId == request.ExamId))
            {
                return Result<StartedExamResponse>.FailureStatusCode("Exam already started by the student", ErrorType.Conflict);
            }

            var Exam = Student.StudentExams.FirstOrDefault(se => se.ExamId == request.ExamId)?.Exam;

            if (Exam == null)
            {
                return Result<StartedExamResponse>.FailureStatusCode("Exam not found", ErrorType.NotFound);
            }

            else if (Exam.StartTime > DateTime.UtcNow)
            {
                return Result<StartedExamResponse>.FailureStatusCode("Exam has not started yet", ErrorType.Conflict);
            }

            else if (Exam.EndTime < DateTime.UtcNow)
            {
                return Result<StartedExamResponse>.FailureStatusCode("Exam has ended", ErrorType.Conflict);
            }

            Student.StudentExams.Add(new StudentExam
            {
                ExamId = request.ExamId,
                TakenAt = DateTime.UtcNow,
                StudentId = user.Id
            });

            studentRepository.Update(user);

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
