using Application.DTOs.Sheets;
using Domain.Entities;
using Domain.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISheetRepository : IRepository<Sheet>
    {
        Task<ICollection<SheetResponse>> GetAllSheetsByCourseAsync(Guid courseId, SheetType sheetType, CancellationToken cancellationToken);
        Task<ICollection<SheetResponse>> GetAllSheetsByVideoAsync(Guid videoId, SheetType sheetType, CancellationToken cancellationToken);
        Task<ICollection<SheetResponse>> GetAllSheetsBySectionAsync(Guid sectionId, SheetType sheetType,CancellationToken cancellationToken);
    }
}
