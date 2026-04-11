using Domain.enums;

namespace Application.Interfaces
{
    public interface ISheetServiceFactory
    {
        ISheetService GetSheetService(SheetTargetType targetType);
    }
}

