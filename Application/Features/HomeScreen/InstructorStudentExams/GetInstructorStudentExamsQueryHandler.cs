using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Centers.Interfaces;
using Application.Features.HomeScreen.DTOs;
using Application.Features.Profiles.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.HomeScreen.InstructorStudentExams
{
    public class GetInstructorStudentExamsQueryHandler(
        IUnitOfWork unitOfWork,
        IInstructorContentScopeService instructorContentScopeService)
        : IRequestHandler<GetInstructorStudentExamsQuery, Result<PaginatedResult<InstructorStudentExamDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IInstructorContentScopeService _instructorContentScopeService = instructorContentScopeService;

        public async Task<Result<PaginatedResult<InstructorStudentExamDto>>> Handle(
            GetInstructorStudentExamsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var profileRepo = _unitOfWork.GetRepository<IProfileRepository>();
                if (!await profileRepo.StudentExistsAsync(request.StudentId, cancellationToken))
                {
                    return Result<PaginatedResult<InstructorStudentExamDto>>.FailureStatusCode(
                        "Student not found.",
                        ErrorType.NotFound);
                }

                if (!await profileRepo.HasSharedContentAsync(
                        request.InstructorId,
                        request.StudentId,
                        cancellationToken))
                {
                    return Result<PaginatedResult<InstructorStudentExamDto>>.FailureStatusCode(
                        "You are not eligible to view this student.",
                        ErrorType.Forbidden);
                }

                var scope = await _instructorContentScopeService.ResolveAsync(
                    request.InstructorId,
                    courseId: null,
                    sectionId: null,
                    cancellationToken);

                var studentId = request.StudentId;
                var courseIds = scope.CourseIds;
                var sectionIds = scope.SectionIds;

                var examsQuery = _unitOfWork.Repository<Exam>()
                    .GetAll(cancellationToken)
                    .AsNoTracking()
                    .Where(e => e.InstructorId == request.InstructorId)
                    .Where(e =>
                        courseIds.Contains(e.CourseId) ||
                        (e.SectionId.HasValue && sectionIds.Contains(e.SectionId.Value)))
                    .OrderByDescending(e => e.StartTime ?? e.CreatedAt)
                    .Select(e => new InstructorStudentExamDto
                    {
                        ExamId = e.Id,
                        Name = e.Name,
                        Description = e.Description,
                        ExamStatus = e.Status,
                        StudentExamStatusResult = e.ExamResults
                            .Where(se => se.StudentId == studentId)
                            .Select(se => se.Status)
                            .FirstOrDefault(),
                        StartTime = e.StartTime,
                        EndTime = e.EndTime,
                        IsTaken = e.ExamResults.Any(se => se.StudentId == studentId),
                        TotalMark = e.TotalMark,
                        NumberOfQuestions = e.NumberOfQuestions,
                        DurationInMinutes = e.DurationInMinutes,
                        IsRandomized = e.IsRandomized,
                        ExamType = e.ExamType,
                        PassMarkPercentage = e.PassMarkPercentage,
                        ObtainedMarks = e.ExamResults
                            .Where(se => se.StudentId == studentId)
                            .Select(se => se.StudentMark)
                            .FirstOrDefault() ?? 0m,
                        TakenAt = e.ExamResults
                            .Where(se => se.StudentId == studentId)
                            .Select(se => se.TakenAt)
                            .FirstOrDefault(),
                        CourseId = e.CourseId,
                        CourseName = e.Course != null ? e.Course.Name : string.Empty,
                        SectionId = e.SectionId,
                        SectionName = e.Section != null ? e.Section.Name : null
                    });

                var page = request.Page < 1 ? 1 : request.Page;
                var pageSize = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 25);
                var result = await examsQuery.ToPaginatedResultAsync(page, pageSize, cancellationToken);
                return Result<PaginatedResult<InstructorStudentExamDto>>.Success(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<PaginatedResult<InstructorStudentExamDto>>.FailureStatusCode(
                    ex.Message,
                    ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<InstructorStudentExamDto>>.FailureStatusCode(
                    $"An error occurred while retrieving student exams: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
