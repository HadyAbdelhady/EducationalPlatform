using Application.DTOs;
using Application.DTOs.Sheets;
using Application.HelperFunctions;
using Application.Interfaces;
using Application.Interfaces.BaseFilters;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.SheetService
{
    public class SheetServiceFactory(
        IUnitOfWork unitOfWork,
        IBaseFilterRegistry<Sheet> sheetFilterRegistry,
        IBaseFilterRegistry<AnswersSheet> answersSheetFilterRegistry) : ISheetServiceFactory
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<Sheet> _sheetFilterRegistry = sheetFilterRegistry;
        private readonly IBaseFilterRegistry<AnswersSheet> _answersSheetFilterRegistry = answersSheetFilterRegistry;

        public ISheetService GetSheetService(SheetTargetType targetType) =>
            targetType switch
            {
                SheetTargetType.Student => new AnswersSheetListingService(_unitOfWork, _answersSheetFilterRegistry),
                SheetTargetType.Course or SheetTargetType.Section or SheetTargetType.Video =>
                    new TargetedSheetListingService(_unitOfWork, _sheetFilterRegistry),
                _ => throw new NotImplementedException(),
            };

        private sealed class TargetedSheetListingService(
            IUnitOfWork unitOfWork,
            IBaseFilterRegistry<Sheet> sheetFilterRegistry) : ISheetService
        {
            private const int PageSize = 10;

            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IBaseFilterRegistry<Sheet> _sheetFilterRegistry = sheetFilterRegistry;

            public async Task<Result<PaginatedResult<SheetResponse>>> GetSheetsAsync(
                Guid targetId,
                SheetType sheetType,
                SheetTargetType targetType,
                GetAllEntityRequestSkeleton requestSkeleton,
                CancellationToken cancellationToken)
            {
                try
                {
                    var existence = await ValidateTargetExistsAsync(targetId, targetType, cancellationToken);
                    if (!existence.ok)
                        return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(existence.message!, ErrorType.NotFound);

                    var skeleton = requestSkeleton ?? new GetAllEntityRequestSkeleton();
                    var query = _unitOfWork.GetRepository<ISheetRepository>()
                        .GetSheetsByTargetQuery(targetId, targetType, sheetType);

                    query = query
                        .ApplyFilters(skeleton.Filters, _sheetFilterRegistry.Filters)
                        .ApplySort(skeleton.SortBy, skeleton.IsDescending, _sheetFilterRegistry.Sorts);

                    var totalCount = await query.CountAsync(cancellationToken);
                    var skip = (skeleton.PageNumber - 1) * PageSize;

                    var items = await query
                        .Skip(skip)
                        .Take(PageSize)
                        .Select(sh => new SheetResponse
                        {
                            Id = sh.Id,
                            Name = sh.Name,
                            SheetUrl = sh.SheetUrl,
                            CreatedAt = sh.CreatedAt,
                            DueDate = sh.DueDate,
                            UpdatedAt = sh.UpdatedAt,
                        })
                        .ToListAsync(cancellationToken);

                    return Result<PaginatedResult<SheetResponse>>.Success(new PaginatedResult<SheetResponse>
                    {
                        Items = items,
                        PageNumber = skeleton.PageNumber,
                        PageSize = PageSize,
                        TotalCount = totalCount,
                    });
                }
                catch (Exception ex)
                {
                    return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(
                        $"An error occurred while retrieving sheets: {ex.Message}",
                        ErrorType.InternalServerError);
                }
            }

            private async Task<(bool ok, string? message)> ValidateTargetExistsAsync(
                Guid targetId,
                SheetTargetType targetType,
                CancellationToken cancellationToken)
            {
                return targetType switch
                {
                    SheetTargetType.Course =>
                        await _unitOfWork.Repository<Course>().AnyAsync(c => c.Id == targetId, cancellationToken)
                            ? (true, null)
                            : (false, "Course not found"),
                    SheetTargetType.Section =>
                        await _unitOfWork.Repository<Section>().AnyAsync(s => s.Id == targetId, cancellationToken)
                            ? (true, null)
                            : (false, "Section not found"),
                    SheetTargetType.Video =>
                        await _unitOfWork.Repository<Video>().AnyAsync(v => v.Id == targetId, cancellationToken)
                            ? (true, null)
                            : (false, "Video not found"),
                    _ => (false, "Invalid target for sheet listing."),
                };
            }
        }

        private sealed class AnswersSheetListingService(
            IUnitOfWork unitOfWork,
            IBaseFilterRegistry<AnswersSheet> answersSheetFilterRegistry) : ISheetService
        {
            private const int PageSize = 10;

            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IBaseFilterRegistry<AnswersSheet> _answersSheetFilterRegistry = answersSheetFilterRegistry;

            public async Task<Result<PaginatedResult<SheetResponse>>> GetSheetsAsync(
                Guid targetId,
                SheetType sheetType,
                SheetTargetType targetType,
                GetAllEntityRequestSkeleton requestSkeleton,
                CancellationToken cancellationToken)
            {
                _ = sheetType;
                _ = targetType;
                try
                {
                    var studentExists = await _unitOfWork.GetRepository<IUserRepository>()
                        .DoesStudentExistAsync(targetId, cancellationToken);
                    if (!studentExists)
                        return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(
                            "Student not found", ErrorType.NotFound);

                    var skeleton = requestSkeleton ?? new GetAllEntityRequestSkeleton();

                    var query = _unitOfWork.Repository<AnswersSheet>()
                        .Find(a => a.StudentId == targetId, cancellationToken, a => a.QuestionsSheet);

                    query = query
                        .ApplyFilters(skeleton.Filters, _answersSheetFilterRegistry.Filters)
                        .ApplySort(skeleton.SortBy, skeleton.IsDescending, _answersSheetFilterRegistry.Sorts);

                    var totalCount = await query.CountAsync(cancellationToken);
                    var skip = (skeleton.PageNumber - 1) * PageSize;

                    var items = await query
                        .Skip(skip)
                        .Take(PageSize)
                        .Select(a => new SheetResponse
                        {
                            Id = a.Id,
                            Name = a.Name,
                            SheetUrl = a.SheetUrl,
                            CreatedAt = a.CreatedAt,
                            UpdatedAt = a.UpdatedAt,
                            DueDate = null,
                            QuestionsSheetId = a.QuestionsSheetId,
                            QuestionsSheetName = a.QuestionsSheet != null ? a.QuestionsSheet.Name : string.Empty,
                            IsApproved = a.IsApproved,
                        })
                        .ToListAsync(cancellationToken);

                    return Result<PaginatedResult<SheetResponse>>.Success(new PaginatedResult<SheetResponse>
                    {
                        Items = items,
                        PageNumber = skeleton.PageNumber,
                        PageSize = PageSize,
                        TotalCount = totalCount,
                    });
                }
                catch (Exception ex)
                {
                    return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(
                        $"An error occurred while retrieving answers sheets: {ex.Message}",
                        ErrorType.InternalServerError);
                }
            }
        }
    }
}
