using Application.Features.Exams.DTOs;
using Application.Common.Interfaces;
using Application.Common;
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
                .GetByIdAsync(
                    request.ExamId,
                    cancellationToken,
                    e => e.Course!,
                    e => e.Course!.EducationYear,
                    e => e.Section!);

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
                var user = await _unitOfWork.Repository<User>()
                    .FirstOrDefaultAsync(
                        predicate: u => u.Id == request.StudentId,
                        cancellationToken: cancellationToken);

                var notStartedDto = new ExamSubmissionDto
                {
                    StudentId = request.StudentId,
                    StudentName = user?.FullName ?? string.Empty,
                    StudentProfilePicture = user?.PersonalPictureUrl ?? string.Empty,
                    Status = ExamResultStatus.NotStarted,
                    ObtainedMarks = 0,
                    TotalMark = exam.TotalMark,
                    PassMarkPercentage = exam.PassMarkPercentage,
                    TakenAt = null,
                    SubmittedAt = default,
                    NumberOfAnswersSubmitted = 0,
                    TotalQuestions = exam.NumberOfQuestions,
                    IsCompleted = false
                };

                return Result<ExamSubmissionDto>.Success(notStartedDto);
            }

            var dto = ExamSubmissionDtoMapping.MapFrom(examResult, ExamSubmissionProjectionContext.FromExam(exam));

            return Result<ExamSubmissionDto>.Success(dto);
        }
    }
}
