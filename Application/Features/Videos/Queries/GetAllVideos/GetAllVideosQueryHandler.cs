using Application.Features.Videos.DTOs;
using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Videos.Queries.GetAllVideos
{
    public class GetAllVideosQueryHandler(IUnitOfWork unitOfWork,
                                          IBaseFilterRegistry<Video> videoFilterRegistry) : IRequestHandler<GetAllVideosQuery, Result<PaginatedResult<VideoByUserIdResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IBaseFilterRegistry<Video> _videoFilterRegistry = videoFilterRegistry;

        public async Task<Result<PaginatedResult<VideoByUserIdResponse>>> Handle(GetAllVideosQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var videos = _unitOfWork.Repository<Video>()
                                        .GetAll(cancellationToken)
                                        .ApplyFilters(request.GetAllEntityRequestSkeleton.Filters, _videoFilterRegistry.Filters)
                                        .ApplySort(request.GetAllEntityRequestSkeleton.SortBy, request.GetAllEntityRequestSkeleton.IsDescending, _videoFilterRegistry.Sorts);

                var videosQuery = videos.Select(v => new VideoByUserIdResponse()
                {
                    Id = v.Id,
                    Name = v.Name,
                    VideoUrl = request.StudentId == null
                        ? v.VideoUrl
                        : (v.Section!.StudentSections.Any(ss => ss.StudentId == request.StudentId)
                           || v.Section.Course!.StudentCourses.Any(sc => sc.StudentId == request.StudentId)
                           ? v.VideoUrl
                           : string.Empty),
                    Description = v.Description,
                    Progress = v.StudentVideos.Where(s => s.StudentId == request.StudentId && s.VideoId == v.Id).Select(s => s.Progress).FirstOrDefault(),
                    NumberOfTutorialSheets = v.Sheets.Count(sh => sh.Type == SheetType.TutorialSheet),
                    NumberOfQuestionsSheets = v.Sheets.Count(sh => sh.Type == SheetType.QuestionSheet),
                    SectionId = v.SectionId,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt ?? v.CreatedAt,
                });

                var paginatedResponse = await videosQuery.ToPaginatedResultAsync(
                    request.GetAllEntityRequestSkeleton.PageNumber,
                    10,
                    cancellationToken);

                return Result<PaginatedResult<VideoByUserIdResponse>>.Success(paginatedResponse);
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<PaginatedResult<VideoByUserIdResponse>>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<VideoByUserIdResponse>>.FailureStatusCode($"An error occurred while retrieving videos: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
