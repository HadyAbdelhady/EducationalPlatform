using Application.DTOs.Exam;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Exams.Query.GetStudentExamResult
{
    public class GetStudentExamResultQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetStudentExamResultQuery, Result<ExamSubmissionDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<ExamSubmissionDto>> Handle(GetStudentExamResultQuery request, CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>()
                .GetByIdAsync(request.ExamId, cancellationToken);

            if (exam == null)
            {
                return Result<ExamSubmissionDto>.FailureStatusCode("Exam not found", ErrorType.NotFound);
            }

            var examResult = await _unitOfWork.Repository<StudentExamResult>()
                .FirstOrDefaultAsync(
                    predicate: er => er.ExamId == request.ExamId && er.StudentId == request.StudentId,
                    cancellationToken: cancellationToken,
                    er => er.Student,
                    er => er.Student.User,
                    er => er.StudentSubmissions,
                    er => er.Exam
                );

            if (examResult == null)
            {
                return Result<ExamSubmissionDto>.FailureStatusCode("Exam result not found for this student", ErrorType.NotFound);
            }

            var dto = new ExamSubmissionDto
            {
                StudentId = examResult.StudentId,
                StudentName = examResult.Student?.User?.FullName ?? string.Empty,
                Status = examResult.Status,
                ObtainedMarks = examResult.StudentMark,
                TotalMark = exam.TotalMark,
                TakenAt = examResult.TakenAt,
                SubmittedAt = examResult.UpdatedAt ?? examResult.CreatedAt,
                NumberOfAnswersSubmitted = examResult.StudentSubmissions?.Count ?? 0,
                TotalQuestions = exam.NumberOfQuestions,
                IsCompleted = examResult.Status == ExamResultStatus.Passed || examResult.Status == ExamResultStatus.Failed
            };

            return Result<ExamSubmissionDto>.Success(dto);
        }
    }
}
