using Application.Interfaces;
using Domain.Entities;
using Domain.enums;
using Infrastructure.Data;

namespace Infrastructure.Repositories
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
