using Domain.enums;

namespace Application.Features.Sheets.Interfaces
{
    public interface ISheetServiceFactory
    {
        ISheetService GetSheetService(SheetTargetType targetType);
    }
}

