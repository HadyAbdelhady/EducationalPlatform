using Application.DTOs.Sections;
using Application.DTOs.Videos;
using Application.Features.Sections.Query.GetSectionByID;
using Application.Features.Sections.Query.GetSectionsForCourse;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SectionRepository(EducationDbContext context) : Repository<Section>(context), ISectionRepository
    {
        public async Task<GetSectionDetailsResponse> GetSectionDetailsResponse(GetSectionByIDQuery Request, CancellationToken cancellationToken)
        {
            GetSectionDetailsResponse? sectionDto = await _context.Sections
                .AsNoTracking()
                .Where(x => x.Id == Request.SectionId)
                .Include(v => v.Videos)
                    .ThenInclude(v => v.StudentVideos)
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
                        VideoUrl = video.VideoUrl,
                        Rating = video.Rating,
                        WatchProgress = video.StudentVideos.Where(sv => sv.VideoId == video.Id && sv.StudentId == Request.UserId)
                                                                           .Select(sv => sv.Progress)
                                                                           .FirstOrDefault(),
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return sectionDto ?? throw new Exception($"Section with ID {Request.SectionId} not found.");
        }

        public async Task<List<GetSectionResponse>> GetSectionInnerData(GetSectionsForCourseQuery Request, CancellationToken cancellationToken)
        {
            return await _context.Sections
                                .AsNoTracking()
                                .Where(s => s.CourseId == Request.CourseId)
                                .Include(v => v.Videos)
                                    .ThenInclude(v => v.StudentVideos)
                                .Select(s => new GetSectionResponse
                                {
                                    SectionId = s.Id,
                                    Name = s.Name,
                                    Description = s.Description,
                                    Price = s.Price,
                                    NumberOfQuestionSheets = s.NumberOfQuestionSheets,
                                    Rating = s.Rating,
                                    CreatedAt = s.CreatedAt,
                                    VideoInfo = s.Videos.Select(v => new VideoResponse
                                    {
                                        VideoId = v.Id,
                                        Name = v.Name,
                                        VideoUrl = v.VideoUrl,
                                        Rating = v.Rating,
                                        WatchProgress = v.StudentVideos.Where(sv => sv.VideoId == v.Id && sv.StudentId == Request.UserId)
                                                                           .Select(sv => sv.Progress)
                                                                           .FirstOrDefault(),
                                    }).ToList()
                                }
                                ).ToListAsync(cancellationToken);

        }

    }

}
