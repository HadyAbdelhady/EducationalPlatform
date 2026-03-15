using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;

namespace Infrastructure.Services.SheetService
{
    public class SheetServiceFactory(IUnitOfWork unitOfWork) : ISheetServiceFactory
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public ISheetService GetSheetService(SheetTargetType targetType) =>
            targetType switch
            {
                SheetTargetType.Course => new CourseSheetService(_unitOfWork),
                SheetTargetType.Section => new SectionSheetService(_unitOfWork),
                SheetTargetType.Video => new VideoSheetService(_unitOfWork),
                SheetTargetType.Student => new AnswersSheetService(_unitOfWork),
                _ => throw new NotImplementedException()
            };
    }

    abstract class SheetServiceBase(IUnitOfWork unitOfWork) : ISheetService
    {
        protected readonly IUnitOfWork UnitOfWork = unitOfWork;

        public abstract Task<Result<PaginatedResult<SheetItem>>> GetSheetsAsync(
            Guid targetId,
            SheetType sheetType,
            CancellationToken cancellationToken);
    }

    sealed class AnswersSheetService(IUnitOfWork unitOfWork) : SheetServiceBase(unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public override async Task<Result<PaginatedResult<SheetItem>>> GetSheetsAsync(Guid targetId, SheetType sheetType, CancellationToken cancellationToken)
        {
            try
            {
                var studentExists = await _unitOfWork.GetRepository<IUserRepository>()
                    .DoesStudentExistAsync(targetId, cancellationToken);
                if (!studentExists)
                    return Result<PaginatedResult<SheetItem>>.FailureStatusCode(
                        "Student not found", ErrorType.NotFound);

                var answersSheets = _unitOfWork.Repository<AnswersSheet>()
                    .Find(x => x.StudentId == targetId, cancellationToken, x => x.QuestionsSheet);
                var list = answersSheets.ToList();

                if (list.Count == 0)
                    return Result<PaginatedResult<SheetItem>>.FailureStatusCode(
                        $"No answers sheets found for student with ID {targetId}.",
                        ErrorType.NotFound);

                var items = list.Select(a => new AllAnswersSheetsByStudentResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    SheetUrl = a.SheetUrl,
                    QuestionsSheetId = a.QuestionsSheetId,
                    QuestionsSheetName = a.QuestionsSheet?.Name ?? string.Empty,
                    IsApproved = a.IsApproved,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                }).ToList();

                return Result<PaginatedResult<SheetItem>>.Success(new PaginatedResult<SheetItem>
                {
                    Items = items,
                    PageNumber = 1,
                    PageSize = list.Count,
                    TotalCount = list.Count
                });
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<SheetItem>>.FailureStatusCode(
                    $"An error occurred while retrieving answers sheets: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }

    sealed class CourseSheetService(IUnitOfWork unitOfWork) : SheetServiceBase(unitOfWork)
    {
        public override async Task<Result<PaginatedResult<SheetItem>>> GetSheetsAsync(
            Guid targetId,
            SheetType sheetType,
            CancellationToken cancellationToken)
        {
            var courseExists = await UnitOfWork.Repository<Course>()
                .AnyAsync(c => c.Id == targetId, cancellationToken);
            if (!courseExists)
                return Result<PaginatedResult<SheetItem>>.FailureStatusCode("Course not found", ErrorType.NotFound);

            var sheets = await UnitOfWork.GetRepository<ISheetRepository>()
                .GetAllSheetsByCourseAsync(targetId, sheetType, cancellationToken);

            var items = sheets.ToList();

            return Result<PaginatedResult<SheetItem>>.Success(new PaginatedResult<SheetItem>
            {
                Items = items,
                PageNumber = 1,
                PageSize = sheets.Count,
                TotalCount = sheets.Count
            });
        }
    }

    sealed class SectionSheetService(IUnitOfWork unitOfWork) : SheetServiceBase(unitOfWork)
    {
        public override async Task<Result<PaginatedResult<SheetItem>>> GetSheetsAsync(
            Guid targetId,
            SheetType sheetType,
            CancellationToken cancellationToken)
        {
            var sectionExists = await UnitOfWork.Repository<Section>()
                .AnyAsync(s => s.Id == targetId, cancellationToken);
            if (!sectionExists)
                return Result<PaginatedResult<SheetItem>>.FailureStatusCode("Section not found", ErrorType.NotFound);

            var sheets = await UnitOfWork.GetRepository<ISheetRepository>()
                .GetAllSheetsBySectionAsync(targetId, sheetType, cancellationToken);

            var items = sheets.ToList();

            return Result<PaginatedResult<SheetItem>>.Success(new PaginatedResult<SheetItem>
            {
                Items = items,
                PageNumber = 1,
                PageSize = sheets.Count,
                TotalCount = sheets.Count
            });
        }
    }

    sealed class VideoSheetService(IUnitOfWork unitOfWork) : SheetServiceBase(unitOfWork)
    {
        public override async Task<Result<PaginatedResult<SheetItem>>> GetSheetsAsync(
            Guid targetId,
            SheetType sheetType,
            CancellationToken cancellationToken)
        {
            var videoExists = await UnitOfWork.Repository<Video>()
                .AnyAsync(v => v.Id == targetId, cancellationToken);
            if (!videoExists)
                return Result<PaginatedResult<SheetItem>>.FailureStatusCode("Video not found", ErrorType.NotFound);

            var sheets = await UnitOfWork.GetRepository<ISheetRepository>()
                .GetAllSheetsByVideoAsync(targetId, sheetType, cancellationToken);

            var items = sheets.ToList();

            return Result<PaginatedResult<SheetItem>>.Success(new PaginatedResult<SheetItem>
            {
                Items = items,
                PageNumber = 1,
                PageSize = sheets.Count,
                TotalCount = sheets.Count
            });
        }
    }
}

