using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Exams.DTOs;
using Domain;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Exams.Query.GetLiveExam
{
    public class GetLiveExamQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetLiveExamQuery, Result<LiveExamResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<LiveExamResponse>> Handle(
            GetLiveExamQuery request,
            CancellationToken cancellationToken)
        {
            var exam = await _unitOfWork.Repository<Exam>()
                .GetByIdAsync(request.ExamId, cancellationToken,
                    e => e.Course!);

            if (exam == null)
                return Result<LiveExamResponse>.FailureStatusCode("Exam not found", ErrorType.NotFound);

            if (exam.InstructorId != request.InstructorId)
                return Result<LiveExamResponse>.FailureStatusCode("Unauthorized", ErrorType.UnAuthorized);

            var now = EgyptTime.UtcNow;
            if (!exam.StartTime.HasValue || !exam.EndTime.HasValue
                || now < exam.StartTime.Value || now > exam.EndTime.Value)
            {
                return Result<LiveExamResponse>.FailureStatusCode(
                    "Exam is not currently live", ErrorType.BadRequest);
            }

            // Resolve enrolled student IDs via Exam navigation
            // Exam -> Course -> StudentCourses / Section -> StudentSections (IEntity-free path using ExamResults)
            // We query StudentExamResult (IEntity) for started students,
            // and use the exam's Course/Section to find all enrolled via a separate IEntity-typed query

            // All results for this exam (started students)
            var results = await _unitOfWork.Repository<StudentExamResult>()
                .GetAll(cancellationToken)
                .AsNoTracking()
                .Where(er => er.ExamId == request.ExamId)
                .Select(er => new
                {
                    er.StudentId,
                    StudentName = er.Student.User.FullName,
                    er.Status,
                    er.TakenAt,
                    er.StudentMark,
                    er.Student.TriedScreenshot
                })
                .ToListAsync(cancellationToken);

            // Get enrolled students through Exam entity navigation (no join table directly)
            // Use ExamQuestions as a side-channel isn't ideal; instead query via Exam -> Course -> StudentCourses
            // StudentCourse is not IEntity, so we go through ExamResult + Exam navigation on IEntity types:
            // Query ExamResults that have Status == NotStarted won't have a row; instead query Exam.CourseId
            // and use the Course entity to find enrolled count via Course.NumberOfStudentsEnrolled.
            // For the full list, we need StudentCourse — but it's not IEntity.
            // ponytail: mark enrolled students without a result as NotStarted using the Course's count.
            // Full per-student list is populated from results only; NotStarted is shown as a count delta.

            int enrolledCount;
            if (exam.SectionId.HasValue)
            {
                var section = await _unitOfWork.Repository<Section>()
                    .GetByIdAsync(exam.SectionId.Value, cancellationToken);
                enrolledCount = section?.NumberOfStudentsEnrolled ?? 0;
            }
            else
            {
                var course = await _unitOfWork.Repository<Course>()
                    .GetByIdAsync(exam.CourseId, cancellationToken);
                enrolledCount = course?.NumberOfStudentsEnrolled ?? 0;
            }

            var resultByStudent = results.ToList();

            var students = resultByStudent.Select(r =>
            {
                int? remaining = null;
                if (r.Status == ExamResultStatus.InProgress && r.TakenAt.HasValue && exam.DurationInMinutes.HasValue)
                {
                    remaining = Math.Max(0,
                        exam.DurationInMinutes.Value - (int)(now - r.TakenAt.Value).TotalMinutes);
                }

                return new LiveExamStudentDto
                {
                    StudentId = r.StudentId,
                    StudentName = r.StudentName,
                    Status = r.Status,
                    TakenAt = r.TakenAt,
                    RemainingMinutes = remaining,
                    AutoScore = (r.Status == ExamResultStatus.Passed || r.Status == ExamResultStatus.Failed)
                        ? r.StudentMark : null,
                    TriedScreenshot = r.TriedScreenshot
                };
            }).ToList();

            int finishedCount = students.Count(s =>
                s.Status == ExamResultStatus.Passed || s.Status == ExamResultStatus.Failed);
            int inProgressCount = students.Count(s => s.Status == ExamResultStatus.InProgress);
            // ponytail: NotStarted = enrolled total minus those with any result row
            int notStartedCount = Math.Max(0, enrolledCount - students.Count);

            return Result<LiveExamResponse>.Success(new LiveExamResponse
            {
                ExamId = exam.Id,
                ExamName = exam.Name,
                CourseName = exam.Course?.Name ?? string.Empty,
                NotStartedCount = notStartedCount,
                InProgressCount = inProgressCount,
                FinishedCount = finishedCount,
                Students = students
            });
        }
    }
}
