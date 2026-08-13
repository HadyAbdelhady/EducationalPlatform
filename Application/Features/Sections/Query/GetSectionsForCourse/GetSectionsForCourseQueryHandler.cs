using Application.Features.Sections.DTOs;
using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sections.Query.GetSectionsForCourse
{
    public class GetSectionsForCourseQueryHandler(IUnitOfWork unitOfWork, IBaseFilterRegistry<Section> sectionFilterRegistry) : IRequestHandler<GetSectionsForCourseQuery, Result<List<SectionDetailsQueryModel>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        private readonly IBaseFilterRegistry<Section> _sectionFilterRegistry = sectionFilterRegistry;


        public async Task<Result<List<SectionDetailsQueryModel>>> Handle(GetSectionsForCourseQuery request, CancellationToken cancellationToken)
        {
            var sections = _unitOfWork.Repository<Section>()
                                                      .GetAll(cancellationToken)
                                                      .Where(s => s.CourseId == request.CourseId)
                                                      .ApplyFilters(request.GetAllEntityRequestSkeleton.Filters, _sectionFilterRegistry.Filters)
                                                      .ApplySort(request.GetAllEntityRequestSkeleton.SortBy, request.GetAllEntityRequestSkeleton.IsDescending, _sectionFilterRegistry.Sorts);


            var response = await sections.Select(s => new SectionDetailsQueryModel
            {
                Section = new SectionData
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    NumberOfVideos = s.NumberOfVideos,
                    NumberOfQuestionSheets = s.NumberOfQuestionSheets,
                    NumberOfExams = s.NumberOfExams,
                    Rating = s.Rating,
                    CourseId = s.CourseId,
                    CreatedAt = s.CreatedAt
                },
                StudentSection = s.StudentSections
                                              .Where(ss => ss.StudentId == request.UserId)
                                              .Select(ss => new StudentSectionData
                                              {
                                                  EnrolledAt = ss.EnrolledAt,
                                                  NumberOfSectionVideosWatched = ss.NumberOfSectionVideosWatched
                                              })
                                              .FirstOrDefault(),
                IsEnrolled = s.StudentSections.Any(ss => ss.StudentId == request.UserId),

                Videos = s.Videos.Select(v => new VideoData
                {
                    Id = v.Id,
                    Name = v.Name,
                    VideoUrl = v.VideoUrl,
                    Rating = v.Rating,
                    StudentVideo = v.StudentVideos
                        .Where(sv => sv.StudentId == request.UserId)
                        .Select(sv => new StudentVideoData
                        {
                            WatchedAt = sv.WatchedAt,
                            Progress = sv.Progress
                        })
                        .FirstOrDefault()
                }).ToList()
            }).ToListAsync(cancellationToken);
            return Result<List<SectionDetailsQueryModel>>.Success(response);
        }
    }
}