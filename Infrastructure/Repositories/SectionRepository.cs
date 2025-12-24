using Application.DTOs.Sections;
using Application.DTOs.Videos;
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
                    NumberOfQuestionSheets = section.NumberOfQuestionSheets,
                    Rating = section.Rating,
                    CreatedAt = section.CreatedAt,
                    UpdatedAt = section.UpdatedAt ?? section.CreatedAt,
                    CourseId = section.CourseId ?? Guid.Empty,
                    Videos = section.Videos.Select(video => new VideoResponse
                    {
                        VideoId = video.Id,
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
