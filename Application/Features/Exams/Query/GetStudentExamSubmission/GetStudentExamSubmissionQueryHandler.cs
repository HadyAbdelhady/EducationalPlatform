using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Exams.DTOs;
using Application.Features.Exams.Interfaces;
using Application.Features.HomeScreen.Interfaces;
using Application.Features.Questions.DTOs;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Exams.Query.GetStudentExamSubmission
{
    public class GetStudentExamSubmissionQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetStudentExamSubmissionQuery, Result<StudentExamSubmissionDetailDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<StudentExamSubmissionDetailDto>> Handle(
            GetStudentExamSubmissionQuery request,
            CancellationToken cancellationToken)
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
                return Result<StudentExamSubmissionDetailDto>.FailureStatusCode("Exam not found", ErrorType.NotFound);
            }

            if (request.CallerUserId != request.StudentId && request.CallerUserId != exam.InstructorId)
            {
                return Result<StudentExamSubmissionDetailDto>.FailureStatusCode(
                    "You are not allowed to view this exam result.",
                    ErrorType.Forbidden);
            }

            if (request.CallerUserId == request.StudentId)
            {
                var enrollmentRepo = _unitOfWork.GetRepository<IStudentEnrollmentRepository>();
                var hasAccess = exam.SectionId.HasValue
                    ? await enrollmentRepo.CanStudentAccessSectionContentAsync(
                        request.StudentId,
                        exam.SectionId.Value,
                        cancellationToken)
                    : await enrollmentRepo.IsStudentEnrolledInCourseAsync(
                        request.StudentId,
                        exam.CourseId,
                        cancellationToken);

                if (!hasAccess)
                {
                    return Result<StudentExamSubmissionDetailDto>.FailureStatusCode(
                        "You are not enrolled in the course or section that contains this exam.",
                        ErrorType.Forbidden);
                }
            }

            var examResult = await _unitOfWork.Repository<StudentExamResult>()
                .FirstOrDefaultAsync(
                    predicate: er => er.ExamId == request.ExamId && er.StudentId == request.StudentId,
                    cancellationToken: cancellationToken,
                    er => er.Student,
                    er => er.Student.User,
                    er => er.StudentSubmissions,
                    er => er.Exam);

            if (examResult == null)
            {
                return Result<StudentExamSubmissionDetailDto>.FailureStatusCode(
                    "Exam result not found for this student",
                    ErrorType.NotFound);
            }

            var examWithQuestions = await _unitOfWork.GetRepository<IExamRepository>()
                .GetExamByIdWithQuestionsAndAnswersAsync(request.ExamId, cancellationToken);

            var chosenByQuestion = (examResult.StudentSubmissions ?? [])
                .GroupBy(s => s.QuestionId)
                .ToDictionary(g => g.Key, g => g.First().ChosenAnswerId);

            var allQuestions = examWithQuestions?.AllQuestionsInExam ?? [];

            return Result<StudentExamSubmissionDetailDto>.Success(new StudentExamSubmissionDetailDto
            {
                Exam = ExamSubmissionDtoMapping.ToExamDetails(exam),
                Submission = ExamSubmissionDtoMapping.MapFrom(
                    examResult,
                    ExamSubmissionProjectionContext.FromExam(exam)),
                Questions = allQuestions
                    .Select(q => Overlay(q, chosenByQuestion))
                    .ToPaginatedResult(request.RequestSkeleton.PageNumber, request.RequestSkeleton.PageSize)
            });
        }

        private static StudentExamQuestionDto Overlay(
            QuestionsInExamWithAnswersResponse question,
            Dictionary<Guid, Guid?> chosenByQuestion)
        {
            chosenByQuestion.TryGetValue(question.Id, out var chosenAnswerId);
            var isCorrect = chosenAnswerId.HasValue
                && question.AllAnswersInExam.Any(a => a.IsCorrect && a.Id == chosenAnswerId.Value);

            return new StudentExamQuestionDto
            {
                Id = question.Id,
                QuestionString = question.QuestionString,
                QuestionImageUrl = question.QuestionImageUrl,
                QuestionMark = question.QuestionMark,
                SectionId = question.SectionId,
                CourseId = question.CourseId,
                AllAnswersInExam = question.AllAnswersInExam,
                ChosenAnswerId = chosenAnswerId,
                IsStudentAnswerCorrect = isCorrect,
                ObtainedMark = isCorrect ? question.QuestionMark ?? 0 : 0
            };
        }
    }
}
