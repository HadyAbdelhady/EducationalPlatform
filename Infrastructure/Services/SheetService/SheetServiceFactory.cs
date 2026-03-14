using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
                _ => throw new NotImplementedException()
            };
    }

    abstract class SheetServiceBase(IUnitOfWork unitOfWork) : ISheetService
    {
        protected readonly IUnitOfWork UnitOfWork = unitOfWork;

        public abstract Task<Result<PaginatedResult<SheetResponse>>> GetSheetsAsync(
            Guid targetId,
            SheetType sheetType,
            CancellationToken cancellationToken);
    }

    sealed class CourseSheetService(IUnitOfWork unitOfWork) : SheetServiceBase(unitOfWork)
    {
        public override async Task<Result<PaginatedResult<SheetResponse>>> GetSheetsAsync(
            Guid targetId,
            SheetType sheetType,
            CancellationToken cancellationToken)
        {
            var courseExists = await UnitOfWork.Repository<Course>()
                .AnyAsync(c => c.Id == targetId, cancellationToken);
            if (!courseExists)
                return Result<PaginatedResult<SheetResponse>>.FailureStatusCode("Course not found", ErrorType.NotFound);

            var sheets = await UnitOfWork.GetRepository<ISheetRepository>()
                .GetAllSheetsByCourseAsync(targetId, sheetType, cancellationToken);

            return Result<PaginatedResult<SheetResponse>>.Success(new PaginatedResult<SheetResponse>
            {
                Items = sheets.ToList(),
                PageNumber = 1,
                PageSize = sheets.Count,
                TotalCount = sheets.Count
            });
        }
    }

    sealed class SectionSheetService(IUnitOfWork unitOfWork) : SheetServiceBase(unitOfWork)
    {
        public override async Task<Result<PaginatedResult<SheetResponse>>> GetSheetsAsync(
            Guid targetId,
            SheetType sheetType,
            CancellationToken cancellationToken)
        {
            var sectionExists = await UnitOfWork.Repository<Section>()
                .AnyAsync(s => s.Id == targetId, cancellationToken);
            if (!sectionExists)
                return Result<PaginatedResult<SheetResponse>>.FailureStatusCode("Section not found", ErrorType.NotFound);

            var sheets = await UnitOfWork.GetRepository<ISheetRepository>()
                .GetAllSheetsBySectionAsync(targetId, sheetType, cancellationToken);

            return Result<PaginatedResult<SheetResponse>>.Success(new PaginatedResult<SheetResponse>
            {
                Items = sheets.ToList(),
                PageNumber = 1,
                PageSize = sheets.Count,
                TotalCount = sheets.Count
            });
        }
    }

    sealed class VideoSheetService(IUnitOfWork unitOfWork) : SheetServiceBase(unitOfWork)
    {
        public override async Task<Result<PaginatedResult<SheetResponse>>> GetSheetsAsync(
            Guid targetId,
            SheetType sheetType,
            CancellationToken cancellationToken)
        {
            var videoExists = await UnitOfWork.Repository<Video>()
                .AnyAsync(v => v.Id == targetId, cancellationToken);
            if (!videoExists)
                return Result<PaginatedResult<SheetResponse>>.FailureStatusCode("Video not found", ErrorType.NotFound);

            var sheets = await UnitOfWork.GetRepository<ISheetRepository>()
                .GetAllSheetsByVideoAsync(targetId, sheetType, cancellationToken);

            return Result<PaginatedResult<SheetResponse>>.Success(new PaginatedResult<SheetResponse>
            {
                Items = sheets.ToList(),
                PageNumber = 1,
                PageSize = sheets.Count,
                TotalCount = sheets.Count
            });
        }
    }
}

