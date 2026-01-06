using Application.Features.Sections.Query.GetSectionsForCourse;
using Application.Features.Sections.Query.GetSectionDetails;
using Microsoft.EntityFrameworkCore;
using Application.DTOs.Sections;
using Application.DTOs.Videos;
using Application.Interfaces;
using Infrastructure.Data;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class SectionRepository(EducationDbContext context) : Repository<Section>(context), ISectionRepository
    {
        public async Task<GetSectionDetailsResponse> GetSectionDetailsResponse(GetSectionDetailsQuery Request, CancellationToken cancellationToken)
        {
            GetSectionDetailsResponse? sectionDto = await _context.Sections
                                                             .AsNoTracking()
                                                             .Where(x => x.Id == Request.SectionId)
                                                             .Include(s => s.StudentSections.Where(ss => ss.StudentId == Request.UserId))
                                                             .Include(s => s.Videos)
                                                                 .ThenInclude(v => v.StudentVideos.Where(sv => sv.StudentId == Request.UserId))
                                                             .Select(section => new GetSectionDetailsResponse
                                                             {
                                                                 SectionId = section.Id,
                                                                 Name = section.Name,
                                                                 Description = section.Description,
                                                                 Price = section.Price,
                                                                 NumberOfVideos = section.Videos.Count,
                                                                 NumberOfSectionVideosWatched = section.StudentSections.FirstOrDefault().NumberOfSectionVideosWatched,
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
                                                                     ProgressData = section.StudentSections.Any()
                                                                                                        ? new VideoProgressData
                                                                                                        {
                                                                                                            EnrolledAt = section.StudentSections.FirstOrDefault().EnrolledAt,
                                                                                                            WatchedAt = video.StudentVideos.FirstOrDefault().WatchedAt,
                                                                                                            Progress = video.StudentVideos.FirstOrDefault().Progress
                                                                                                        }
                                                                                                        : null
                                                                     //ProgressData = new VideoProgressData()
                                                                 }).ToList()
                                                             })
                                                             .FirstOrDefaultAsync(cancellationToken);

            return sectionDto ?? throw new Exception($"Section with ID {Request.SectionId} not found.");
        }

        public async Task<List<GetSectionDetailsResponse>> GetSectionInnerData(GetSectionsForCourseQuery Request, CancellationToken cancellationToken)
        {
            return await _context.Sections
                                .AsNoTracking()
                                .Where(s => s.CourseId == Request.CourseId)
                                .Include(v => v.Videos)
                                    .ThenInclude(v => v.StudentVideos)
                                .Select(s => new GetSectionDetailsResponse
                                {
                                    SectionId = s.Id,
                                    Name = s.Name,
                                    Description = s.Description,
                                    Price = s.Price,
                                    NumberOfQuestionSheets = s.NumberOfQuestionSheets,
                                    Rating = s.Rating,
                                    CreatedAt = s.CreatedAt,
                                    Videos = s.Videos.Select(v => new VideoResponse
                                    {
                                        VideoId = v.Id,
                                        Name = v.Name,
                                        VideoUrl = v.VideoUrl,
                                        Rating = v.Rating,
                                        //WatchProgress = v.StudentVideos.Where(sv => sv.VideoId == v.Id && sv.StudentId == Request.UserId)
                                        //                                   .Select(sv => sv.Progress)
                                        //                                   .FirstOrDefault(),
                                    }).ToList(),
                                    CourseId = Request.CourseId,
                                    NumberOfVideos = s.NumberOfVideos,
                                    UpdatedAt = s.UpdatedAt ?? s.CreatedAt,
                                }
                                )
                                .ToListAsync(cancellationToken);

        }

        //public async Task<GetSectionDetailsResponse> GetEnrolledSectionDetails(GetSectionDetailsQuery Request, CancellationToken cancellationToken)
        //{
        //    return await _context.StudentSections
        //             .AsNoTracking()
        //             .Where(sec => sec.StudentId == Request.UserId)
        //                 .Include(Sec => Sec.Section)
        //            .Select(s => new GetSectionDetailsResponse
        //            {
        //                SectionId = s.Section.Id,
        //                Name = s.Section.Name,
        //                Description = s.Section.Description,
        //                Price = s.Section.Price,
        //                NumberOfVideos = s.Section.Videos.Count,
        //                NumberOfQuestionSheets = s.Section.NumberOfQuestionSheets,
        //                Rating = s.Section.Rating,
        //                CreatedAt = s.Section.CreatedAt,
        //                UpdatedAt = s.Section.UpdatedAt ?? s.Section.CreatedAt,
        //                CourseId = s.Section.CourseId ?? Guid.Empty,
        //                Videos = s.Section.Videos.Select(video => new VideoResponse
        //                {
        //                    VideoId = video.Id,
        //                    Name = video.Name,
        //                    VideoUrl = video.VideoUrl,
        //                    Rating = video.Rating,
        //                    WatchProgress = video.StudentVideos.Where(sv => sv.VideoId == video.Id && sv.StudentId == Request.UserId)
        //                                                                        .Select(sv => sv.Progress)
        //                                                                        .FirstOrDefault(),
        //                }).ToList()
        //            })
        //             .FirstOrDefaultAsync(cancellationToken) ?? throw new Exception($"Student Do not have Section enrollement");

        //}
    }

}
