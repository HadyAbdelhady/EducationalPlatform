using Infrastructure.Common;
using Application.Common.Interfaces;
using Application.Features.Sheets.Interfaces;
using Domain.Entities;
using Domain.enums;
using Infrastructure.Common.Data;

namespace Infrastructure.Features.Sheets
{
    public class SheetRepository(EducationDbContext _context) : Repository<Sheet>(_context), ISheetRepository
    {
        public IQueryable<Sheet> GetSheetsByTargetQuery(Guid targetId, SheetTargetType targetType, SheetType sheetType)
        {
            var query = _context.Sheets.Where(sh => sh.Type == sheetType);

            return targetType switch
            {
                SheetTargetType.Course => query.Where(sh => sh.CourseId == targetId),
                SheetTargetType.Section => query.Where(sh => sh.SectionId == targetId),
                SheetTargetType.Video => query.Where(sh => sh.VideoId == targetId),
                SheetTargetType.Student => throw new ArgumentException("Use AnswersSheet queries for student target.", nameof(targetType)),
                _ => throw new ArgumentOutOfRangeException(nameof(targetType)),
            };
        }
    }
}
