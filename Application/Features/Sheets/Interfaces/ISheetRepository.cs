using Application.Common.Interfaces;
using Domain.Entities;
using Domain.enums;

namespace Application.Features.Sheets.Interfaces
{
    public interface ISheetRepository : IRepository<Sheet>
    {
        IQueryable<Sheet> GetSheetsByTargetQuery(Guid targetId, SheetTargetType targetType, SheetType sheetType);
    }
}
