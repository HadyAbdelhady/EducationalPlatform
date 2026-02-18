using Application.DTOs.Sheets;
using Application.Interfaces;
using Domain.Entities;
using Domain.enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SheetRepository(EducationDbContext _context) : Repository<Sheet>(_context), ISheetRepository
    {
        public async Task<ICollection<SheetResponse>> GetAllSheetsByCourseAsync(Guid courseId, SheetType sheetType, CancellationToken cancellationToken)
        {
            return await _context.Sheets
                .Where(sh => sh.CourseId == courseId && sh.Type == sheetType)
                .Select(sh => new SheetResponse
                {
                    Id = sh.Id,
                    Name = sh.Name,
                    SheetUrl = sh.SheetUrl,
                    CreatedAt = sh.CreatedAt,
                    DueDate = sh.DueDate,
                    UpdatedAt = sh.UpdatedAt   
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<ICollection<SheetResponse>> GetAllSheetsByVideoAsync(Guid videoId, SheetType sheetType, CancellationToken cancellationToken)
        {
            return await _context.Sheets
                .Where(sh => sh.VideoId == videoId && sh.Type == sheetType)
                .Select(sh => new SheetResponse
                {
                    Id = sh.Id,
                    Name = sh.Name,
                    SheetUrl = sh.SheetUrl,
                    CreatedAt = sh.CreatedAt,
                    DueDate = sh.DueDate,
                    UpdatedAt = sh.UpdatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<ICollection<SheetResponse>> GetAllSheetsBySectionAsync(Guid sectionId, SheetType sheetType, CancellationToken cancellationToken)
        {
            return await _context.Sheets
                .Where(sh => sh.SectionId == sectionId && sh.Type == sheetType)
                .Select(sh => new SheetResponse
                {
                    Id = sh.Id,
                    Name = sh.Name,
                    SheetUrl = sh.SheetUrl,
                    CreatedAt = sh.CreatedAt,
                    DueDate = sh.DueDate,
                    UpdatedAt = sh.UpdatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
