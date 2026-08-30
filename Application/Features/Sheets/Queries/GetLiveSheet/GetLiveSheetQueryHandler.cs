using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Sheets.DTOs;
using Domain;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sheets.Queries.GetLiveSheet
{
    public class GetLiveSheetQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetLiveSheetQuery, Result<LiveSheetResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<LiveSheetResponse>> Handle(
            GetLiveSheetQuery request,
            CancellationToken cancellationToken)
        {
            var sheet = await _unitOfWork.Repository<Sheet>()
                .GetByIdAsync(request.SheetId, cancellationToken,
                    s => s.Course!, s => s.Section!, s => s.Section!.Course);

            if (sheet == null)
                return Result<LiveSheetResponse>.FailureStatusCode("Sheet not found", ErrorType.NotFound);

            if (sheet.InstructorId != request.InstructorId)
                return Result<LiveSheetResponse>.FailureStatusCode("Unauthorized", ErrorType.UnAuthorized);

            var now = EgyptTime.UtcNow;

            // Live = due date is in the future
            if (!sheet.DueDate.HasValue || sheet.DueDate.Value < now)
            {
                return Result<LiveSheetResponse>.FailureStatusCode(
                    "Sheet is not currently live (due date has passed or is not set)", ErrorType.BadRequest);
            }

            // Enrolled count from cached column (IEntity-safe, no join table)
            int enrolledCount;
            if (sheet.SectionId.HasValue)
            {
                var section = await _unitOfWork.Repository<Section>()
                    .GetByIdAsync(sheet.SectionId.Value, cancellationToken);
                enrolledCount = section?.NumberOfStudentsEnrolled ?? 0;
            }
            else if (sheet.CourseId.HasValue)
            {
                var course = await _unitOfWork.Repository<Course>()
                    .GetByIdAsync(sheet.CourseId.Value, cancellationToken);
                enrolledCount = course?.NumberOfStudentsEnrolled ?? 0;
            }
            else
            {
                enrolledCount = 0;
            }

            // Submissions for this sheet — AnswersSheet is IEntity-compatible via Sheet
            var submissions = await _unitOfWork.Repository<AnswersSheet>()
                .GetAll(cancellationToken)
                .AsNoTracking()
                .Where(a => a.QuestionsSheetId == sheet.Id)
                .Select(a => new LiveSheetStudentDto
                {
                    StudentId = a.StudentId,
                    StudentName = a.Student.User.FullName,
                    HasSubmitted = true,
                    IsApproved = a.IsApproved,
                    SubmittedAt = a.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var courseName = sheet.Course?.Name
                ?? sheet.Section?.Course?.Name
                ?? string.Empty;

            int submittedCount = submissions.Count;
            // ponytail: NotSubmitted = enrolled - submitted (count only; student list is submission side)
            int notSubmittedCount = Math.Max(0, enrolledCount - submittedCount);

            return Result<LiveSheetResponse>.Success(new LiveSheetResponse
            {
                SheetId = sheet.Id,
                SheetName = sheet.Name,
                CourseName = courseName,
                DueDate = sheet.DueDate,
                EnrolledCount = enrolledCount,
                SubmittedCount = submittedCount,
                NotSubmittedCount = notSubmittedCount,
                Students = submissions
            });
        }
    }
}
