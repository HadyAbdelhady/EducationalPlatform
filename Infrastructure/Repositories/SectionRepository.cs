using Application.Features.Sections.Query.GetSectionsForCourse;
using Application.Features.Sections.Query.GetSectionDetails;
using Microsoft.EntityFrameworkCore;
using Application.DTOs.Sections;
using Application.Interfaces;
using Infrastructure.Data;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class SectionRepository(EducationDbContext context) : Repository<Section>(context), ISectionRepository
    {
        public async Task<SectionDetailsQueryModel> GetSectionDetailsResponse(GetSectionDetailsQuery Request, CancellationToken cancellationToken)
        {
            return await _context.Sections
                                   .AsNoTracking()
                                   .Where(s => s.Id == Request.SectionId)
                                   .Select(s => new SectionDetailsQueryModel
                                   {
                                       Section = s,
                                       StudentSection = s.StudentSections
                                           .Where(ss => ss.StudentId == Request.UserId)
                                           .Select(ss => new StudentSectionData
                                           {
                                               EnrolledAt = ss.EnrolledAt,
                                               NumberOfSectionVideosWatched = ss.NumberOfSectionVideosWatched
                                           })
                                           .FirstOrDefault(),

                                       Videos = s.Videos.Select(v => new VideoData
                                       {
                                           Id = v.Id,
                                           Name = v.Name,
                                           VideoUrl = v.VideoUrl,
                                           Rating = v.Rating,
                                           StudentVideo = v.StudentVideos
                                               .Where(sv => sv.StudentId == Request.UserId)
                                               .Select(sv => new StudentVideoData
                                               {
                                                   WatchedAt = sv.WatchedAt,
                                                   Progress = sv.Progress
                                               })
                                               .FirstOrDefault()
                                       }).ToList()
                                   })
                                   .FirstOrDefaultAsync(cancellationToken) ?? throw new Exception("Section not found");
        }

        public async Task<List<SectionDetailsQueryModel>> GetSectionInnerData(GetSectionsForCourseQuery Request, CancellationToken cancellationToken)
        {
            return await _context.Sections
                                .AsNoTrackingWithIdentityResolution()
                                .Where(s => s.CourseId == Request.CourseId)
                                .Select(s => new SectionDetailsQueryModel
                                {
                                    Section = s,
                                    StudentSection = s.StudentSections
                                           .Where(ss => ss.StudentId == Request.UserId)
                                           .Select(ss => new StudentSectionData
                                           {
                                               EnrolledAt = ss.EnrolledAt,
                                               NumberOfSectionVideosWatched = ss.NumberOfSectionVideosWatched
                                           })
                                           .FirstOrDefault(),

                                    Videos = s.Videos.Select(v => new VideoData
                                    {
                                        Id = v.Id,
                                        Name = v.Name,
                                        VideoUrl = v.VideoUrl,
                                        Rating = v.Rating,
                                        StudentVideo = v.StudentVideos
                                            .Where(sv => sv.StudentId == Request.UserId)
                                            .Select(sv => new StudentVideoData
                                            {
                                                WatchedAt = sv.WatchedAt,
                                                Progress = sv.Progress
                                            })
                                            .FirstOrDefault()
                                    }).ToList()
                                })
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
