using Application.DTOs.Section;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SectionRepository(EducationDbContext context) : Repository<Domain.Entities.Section>(context), ISectionRepository
    {
        public async Task<GetSectionDetailsResponse> GetSectionDetailsResponse(Guid sectionId, CancellationToken cancellationToken)
        {
            GetSectionDetailsResponse? sectionDto = await _context.Sections
                .AsNoTracking()
                .Where(x => x.Id == sectionId)
                .Select(section => new GetSectionDetailsResponse
                {
                    SectionId = section.Id,
                    Name = section.Name,
                    Description = section.Description,
                    Price = section.Price,
                    NumberOfVideos = section.Videos.Count,
                    Rating = section.Rating,
                    CreatedAt = section.CreatedAt,
                    UpdatedAt = section.UpdatedAt,
                    CourseId = section.CourseId ?? Guid.Empty,
                    Videos = section.Videos.Select(video => new VideoInfo
                    {
                        Id = video.Id,
                        Name = video.Name,
                        Description = video.Description,
                        VideoUrl = video.VideoUrl,
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return sectionDto ?? throw new Exception($"Section with ID {sectionId} not found.");
        }
    }

}
