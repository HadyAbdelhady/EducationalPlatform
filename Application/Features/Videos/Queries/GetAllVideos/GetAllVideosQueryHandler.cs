using Application.DTOs.Videos;
using Application.HelperFunctions;
using Application.Interfaces;
using Application.Interfaces.BaseFilters;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

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

                var response = videos.Select(v => new VideoByUserIdResponse()
                {
                    Id = v.Id,
                    Name = v.Name,
                    VideoUrl = v.VideoUrl,
                    Description = v.Description,
                    NumberOfTutorialSheets = v.Sheets.Where(sh => sh.Type == SheetType.TutorialSheet).ToList().Count,
                    NumberOfQuestionsSheets = v.Sheets.Where(sh => sh.Type == SheetType.QuestionSheet).ToList().Count,
                    SectionId = v.SectionId,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt ?? v.CreatedAt,
                }).ToList();

                int pageSize = 10;
                int skip = (request.GetAllEntityRequestSkeleton.PageNumber - 1) * pageSize;
                var paginatedResponse = response.Skip(skip).Take(pageSize).ToList();

                return Result<PaginatedResult<VideoByUserIdResponse>>.Success(new PaginatedResult<VideoByUserIdResponse>
                {
                    Items = paginatedResponse,
                    PageNumber = request.GetAllEntityRequestSkeleton.PageNumber,
                    PageSize = pageSize,
                    TotalCount = response.Count
                });
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
